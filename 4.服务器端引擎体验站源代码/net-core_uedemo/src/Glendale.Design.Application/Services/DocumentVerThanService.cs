using Glendale.Design.Dtos.DocumentVerThan;
using Glendale.Design.Dtos.ModelTree;
using Glendale.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Glendale.Design.Core.Extensions;
using Glendale.Design.Dtos.DocumentVer;

namespace Glendale.Design.Services
{
    public class DocumentVerThanService : CrudAppService<DocumentVerThan, DocumentVerThanDto, DocumentVerThanListDto, Guid, GetListDocumentVerThanInput, DocumentVerThanCreateUpdateDto, DocumentVerThanCreateUpdateDto>, IDocumentVerThanService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IModelTreeAppService _modelTreeAppService;
        private readonly IRepository<ModelProperty, int> _modelPropertyRepository;
        private readonly IRepository<DocumentVersion,Guid> _documentVerRepository;
        public DocumentVerThanService(IRepository<DocumentVerThan, Guid> repository,IModelTreeAppService modelTreeAppService, 
            IRepository<ModelProperty, int> modelPropertyRepository,IRepository<DocumentVersion, Guid> documentVerRepository): base(repository)
        {
            _modelTreeAppService = modelTreeAppService;
            _modelPropertyRepository = modelPropertyRepository;
            _documentVerRepository = documentVerRepository;
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Task<DocumentVerThanDto> CreateAsync(DocumentVerThanCreateUpdateDto input)
        {
            //input.AddMetadata = null;
            //input.UpdateMetadata = null;
            //input.DeleteMetadata = null;
            return base.CreateAsync(input);
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="destinationId"></param>
        /// <returns></returns>
        public async Task<DocumentVerThanDto> GetEntity(Guid sourceId, Guid destinationId)
        {
           var entity= Repository.Where(p => p.SourceDocVerId == sourceId && p.DestinationDocVerId == destinationId).FirstOrDefault();

            var dto=await MapToGetOutputDtoAsync(entity);
            if (dto == null)
            {
                return null;
            }
            dto.SourceDocVer = ObjectMapper.Map<DocumentVersion, DocumentVersionDto>(await _documentVerRepository.GetAsync(dto.SourceDocVerId));
            dto.DestinationDocVer = ObjectMapper.Map<DocumentVersion, DocumentVersionDto>(await _documentVerRepository.GetAsync(dto.DestinationDocVerId));

            dto.AddMetadatas = SerializeExtensions.FromJson<List<MetadataCreateDto>>(dto.AddMetadata);
            dto.UpdateMetadatas = SerializeExtensions.FromJson<List<MetadataCreateDto>>(dto.UpdateMetadata);
            dto.DeleteMetadatas = SerializeExtensions.FromJson<List<MetadataCreateDto>>(dto.DeleteMetadata);
            dto.AddMetadata = null;
            dto.UpdateMetadata = null;
            dto.DeleteMetadata = null;
            return dto;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<DocumentVerThanDto> GetAsync(Guid id)
        {
            var dto=await base.GetAsync(id);
            dto.SourceDocVer = ObjectMapper.Map<DocumentVersion, DocumentVersionDto>(await _documentVerRepository.GetAsync(dto.SourceDocVerId));
            dto.DestinationDocVer =ObjectMapper.Map<DocumentVersion,DocumentVersionDto>(await _documentVerRepository.GetAsync(dto.DestinationDocVerId));
            
            dto.AddMetadatas = SerializeExtensions.FromJson<List<MetadataCreateDto>>(dto.AddMetadata);
            dto.UpdateMetadatas = SerializeExtensions.FromJson<List<MetadataCreateDto>>(dto.UpdateMetadata);
            dto.DeleteMetadatas = SerializeExtensions.FromJson<List<MetadataCreateDto>>(dto.DeleteMetadata);
            dto.AddMetadata = null;
            dto.UpdateMetadata = null;
            dto.DeleteMetadata = null;
            return dto;
        }
        
        [RemoteService(false)]
        public override Task<DocumentVerThanDto> UpdateAsync(Guid id, DocumentVerThanCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        [RemoteService(false)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Task<PagedResultDto<DocumentVerThanListDto>> GetListAsync(GetListDocumentVerThanInput input)
        {            
            return base.GetListAsync(input);
        }

        /// <summary>
        /// 开始比对
        /// </summary>
        /// <param name="id">比对表主键Id</param>
        /// <param name="glid">旧版图层glid</param>
        /// <param name="newglid">新版图层glid</param>
        /// <returns></returns>
        public async Task<MetadataListDto> StartThan(Guid id, string glid,string newglid)
        {
            var guid = String.Format("{0}#{1}", glid, newglid);
            var result = new MetadataListDto();
            var entity = await base.GetAsync(id);
            var addList = SerializeExtensions.FromJson<List<MetadataCreateDto>>(entity.AddMetadata);
            var updateList = SerializeExtensions.FromJson<List<MetadataCreateDto>>(entity.UpdateMetadata);
            var deleteList = SerializeExtensions.FromJson<List<MetadataCreateDto>>(entity.DeleteMetadata);
            addList = addList != null ? addList : new List<MetadataCreateDto>();
            updateList = updateList != null ? updateList : new List<MetadataCreateDto>();
            deleteList = deleteList != null ? deleteList : new List<MetadataCreateDto>();

            entity.SourceDocVer = ObjectMapper.Map<DocumentVersion, DocumentVersionDto>(await _documentVerRepository.GetAsync(entity.SourceDocVerId));
            entity.DestinationDocVer = ObjectMapper.Map<DocumentVersion, DocumentVersionDto>(await _documentVerRepository.GetAsync(entity.DestinationDocVerId));
            //源模型构件
            var arraySource = await _modelTreeAppService.GetTreeChildId(entity.SourceDocVer.ModelName, glid);
            //目标模型构件
            var arrayDestination = await _modelTreeAppService.GetTreeChildId(entity.DestinationDocVer.ModelName, newglid);

            #region 若已经保存，则返回保存的；若未保存，则返回比对结果
            var addCurrent = addList.FirstOrDefault(p => p.Guid == guid);
            if (addCurrent != null)
            {
                result.AddMetadata = addCurrent;
            }
            else
            {
                //新增的构件
                result.AddMetadata = new MetadataCreateDto()
                {
                    Guid = guid,
                    ExternalIdList = arrayDestination.Except(arraySource).ToArray(),//Des有，Source没有（新增）
                };
            } 
            var updateCurrent=updateList.FirstOrDefault(p => p.Guid == guid);
            if (updateCurrent != null)
            {
                result.UpdateMetadata = updateCurrent;
            }
            else
            {
                //修改的构件
                var someIds = arrayDestination.Intersect(arraySource).ToArray();//两者都有
                result.UpdateMetadata = new MetadataCreateDto()
                {
                    Guid = guid,
                    ExternalIdList = await GetExternalIdList(someIds, entity.SourceDocVer.ModelName, entity.DestinationDocVer.ModelName)
                };
            }
            var deleteCurrent=deleteList.FirstOrDefault(p => p.Guid == guid);
            if (deleteCurrent != null)
            {
                result.DeleteMetadata = deleteCurrent;
            }
            else
            {
                //删除的构件
                result.DeleteMetadata = new MetadataCreateDto()
                {
                    Guid = guid,
                    ExternalIdList = arraySource.Except(arrayDestination).ToArray(),//Source有，Des没有（删除）
                };
            }
            #endregion
            #region 分别返回选中楼层的构件集合
            result.SourceMetadata = new MetadataCreateDto()
            {
                Guid = glid,
                ExternalIdList = arraySource.ToArray(),
            };
            result.DestinationMetadata = new MetadataCreateDto()
            {
                Guid = newglid,
                ExternalIdList = arrayDestination.ToArray(),
            };
            #endregion
            return result;
        }

        /// <summary>
        /// 计算update构件Id
        /// </summary>
        /// <param name="someIds">两个版本都存在的构件Id</param>
        /// <param name="modelNameSource"></param>
        /// <param name="modelNameDestination"></param>
        /// <returns></returns>
        private async Task<string[]> GetExternalIdList(string[] someIds, string modelNameSource, string modelNameDestination)
        {
            List<string> result = new List<string>();
            var ProListSource =await _modelPropertyRepository.GetListAsync(p => p.ModelName == modelNameSource && someIds.Contains(p.ExternalId));
            var ProListDestination = await _modelPropertyRepository.GetListAsync(p => p.ModelName == modelNameDestination && someIds.Contains(p.ExternalId));

            for (int i = 0; i < someIds.Length; i++)
            {
                var propertySource = ProListSource.Where(p => p.ExternalId == someIds[i]).Select(p => p.Value).ToArray();
                var propertyDestination = ProListDestination.Where(p => p.ExternalId == someIds[i]).Select(p => p.Value).ToArray();
                //比较属性的数量是否不同
                if (propertySource.Length != propertyDestination.Length)
                {
                    result.Add(someIds[i]);
                    continue;
                }      
                //如果属性数量一致，比较属性值是否相同
                if(!Enumerable.SequenceEqual(propertySource, propertyDestination))
                {
                    result.Add(someIds[i]);                         
                }                 
            }
            return result.ToArray();        
        }

        /// <summary>
        /// 保存构件变更
        /// </summary>
        /// <param name="id">比对表主键Id</param>
        /// <param name="method">1=新增 2=修改 3=删除</param>
        /// <param name="metadata">构件列表(Guid=旧版plid#新版plid))</param>
        /// <returns></returns>
        public async Task<bool> MetadataSave(Guid id, int method, MetadataCreateDto metadata)
        {
            var entity = await Repository.GetAsync(id);
            var addList= SerializeExtensions.FromJson<List<MetadataCreateDto>>(entity.AddMetadata);
            var updateList= SerializeExtensions.FromJson<List<MetadataCreateDto>>(entity.UpdateMetadata);
            var deleteList= SerializeExtensions.FromJson<List<MetadataCreateDto>>(entity.DeleteMetadata);
            addList = addList != null? addList : new List<MetadataCreateDto>();
            updateList = updateList != null? updateList : new List<MetadataCreateDto>();
            deleteList = deleteList != null? deleteList : new List<MetadataCreateDto>();
            if (method == 1)
            {
                if (addList.Any())
                {
                    var current = addList.FirstOrDefault(p => p.Guid == metadata.Guid);
                    if (current != null)
                    {
                        addList.Remove(current);  //移除旧的                    
                    }
                }
                addList.Add(metadata);//添加新的
            }
            else if (method == 2)
            {
                if (updateList.Any())
                {
                    var current = updateList.FirstOrDefault(p => p.Guid == metadata.Guid);
                    if (current != null)
                    {
                        updateList.Remove(current);  //移除旧的                    
                    }
                }
                updateList.Add(metadata);//添加新的
            }
            else if (method == 3)
            {
                if (deleteList.Any())
                {
                    var current = deleteList.FirstOrDefault(p => p.Guid == metadata.Guid);
                    if (current != null)
                    {
                        deleteList.Remove(current);  //移除旧的                    
                    }
                }
                deleteList.Add(metadata);//添加新的
            }
            else
            {
                throw new UserFriendlyException("参数method不是有效的值");
            }
            entity.AddMetadata = SerializeExtensions.ToJson(addList);
            entity.UpdateMetadata = SerializeExtensions.ToJson(updateList);
            entity.DeleteMetadata = SerializeExtensions.ToJson(deleteList);
            await Repository.UpdateAsync(entity);
            return true;
        }

    }
}

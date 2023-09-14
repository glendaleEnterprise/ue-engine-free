// eslint-disable-next-line
import {
  UserLayout,
  BasicLayout,
  BlankLayout,
  PageView
} from '@/layouts'
/**
 * 主路由
*/
export const mainRouterMap = [{
  path: '/',
  name: 'index',
  component: BasicLayout,
  meta: {
    title: '首页'
  },
  redirect: '/project',
  children: [
    {
      path: '/project',
      name: 'project',
      component: () => import('@/views/project/Index'),
      meta: {
        title: '项目列表',
        icon: 'bars',
        keepAlive: false,
        permission: ['Design.Base.Projects']
      }
    },
    {
      path: '/system',
      name: 'system',
      component: PageView,
      redirect: '/system/user',
      meta: {
        title: '基础设置',
        icon: 'setting',
        keepAlive: false,
        permission: ['Design.System']
      },
      children: [{
        path: '/system/user',
        name: 'system.user',
        component: () => import('@/views/system/User'),
        meta: {
          title: '用户管理',
          icon: 'team',
          keepAlive: false,
          permission: ['Design.System.Users']
        }
      },
      // {
      //   path: '/system/organizationunit',
      //   name: 'system.organizationunit',
      //   component: () => import('@/views/system/OrganizationUnit'),
      //   meta: {
      //     title: '组织机构',
      //     icon: 'cluster',
      //     keepAlive: false,
      //     permission: ['Design.System.OrganizationUnits']
      //   }
      // },
      // {
      //   path: '/system/role',
      //   name: 'system.role',
      //   component: () => import('@/views/system/Role'),
      //   meta: {
      //     title: '角色管理',
      //     icon: 'user',
      //     keepAlive: false,
      //     permission: ['Design.System.Roles']
      //   }
      // },
      // {
      //   path: '/system/dictionary',
      //   name: 'system.dictionary',
      //   component: () => import('@/views/system/Dictionary'),
      //   meta: {
      //     title: '基础数据维护',
      //     icon: 'tool',
      //     keepAlive: false,
      //     permission: ['Design.System.Dictionary']
      //   }
      // },
      {
        path: '/system/logs',
        name: 'system.logs',
        component: () => import('@/views/system/Logs'),
        meta: {
          title: '日志查询',
          icon: 'file',
          keepAlive: false,
          permission: ['Design.System.LogData']
        }
      },
      ]
    },
    {
      path: '/business',
      component: BlankLayout,
      name: 'business',
      project: true,
      hidden: true,
      meta: {
        title: '项目管理',
        icon: 'project',
        keepAlive: false,
        // permission: ['Design.Business.Home']
      },
      children: [
        // {
        //   path: '/business/home',
        //   name: 'business.home',
        //   component: () => import('@/views/project/Home'),
        //   meta: {
        //     title: '项目首页',
        //     icon: 'home',
        //     keepAlive: false,
        //     project: true,
        //     permission: ['Design.Business.Home']
        //   }
        // },
        {
          path: '/business/folder',
          name: 'business.folder',
          component: () => import('@/views/project/Folder'),
          meta: {
            title: '模型目录',
            icon: 'folder-add',
            keepAlive: false,
            project: true,
            permission: ['Design.Business.Folder']
          }
        },
        {
          path: '/business/model',
          name: 'business.model',
          component: () => import('@/views/document/Model'),
          meta: {
            title: '模型列表',
            icon: 'border',
            keepAlive: false,
            project: true,
            permission: ['Design.Business.Documents']
          }
        },
        {
          path: '/business/combine',
          name: 'business.combine',
          component: () => import('@/views/combine/List'),
          meta: {
            title: '合模列表',
            icon: 'block',
            keepAlive: false,
            project: true,
            permission: ['Design.Business.Combine']
          }
        },
        {
          path: '/business/share',
          name: 'business.share',
          component: () => import('@/views/share/Shares'),
          meta: {
            title: '分享管理',
            icon: 'share-alt',
            keepAlive: false,
            project: true,
            permission: ['Design.Business.Share']
          }
        },
      ]
    }
  ]
},
{
  path: '*',
  redirect: '/project',
  hidden: true
}
]

/**
 * 基础路由
 * @type { *[] }
 */
export const constantRouterMap = [{
  path: '/login',
  component: UserLayout,
  redirect: '/login',
  hidden: true,
  children: [{
    path: '/login',
    name: 'login',
    component: () => import('@/views/user/Login')
  },
  ]
},
{
  path: '/404',
  name: '404',
  component: () => import('@/views/exception/404')
},
{
  path: '/sharelink/:id',
  name: 'sharelink',
  component: () => import('@/views/share/ShareLink')
},
{
  path: '/model/:id',
  name: 'model',
  component: () => import('@/views/bim/Index')
},
{
  path: '/combine/:id',
  name: 'combine',
  component: () => import('@/views/bim/Index')
},
{
  path: '/clamping/index',
  name: 'clamping',
  component: () => import('@/views/combine/Index')
},
{
  path: '/expired',
  name: 'expired',
  component: () => import('@/views/exception/Expired')
},
{
  path: '/agreement',
  name: 'agreement',
  component: () => import('@/views/exception/agreement')
},
]


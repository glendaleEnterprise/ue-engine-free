using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Dtos.Roaming
{
	/// <summary>
	/// 自定义视点漫游记录
	/// </summary>
	public class ViewPortPointCreateDto
    {
       // "world":{ "location":{"x":-13444.863285,"y":-6352.97705,"z":368.6201477}, "rotation":{"x":0.000043426694,"y":-12.01570894,"z":-25.33145713} }         
        public World world{get;set;}
    }

    public class World
    {
        public Point Location { get; set; }
        public Point Rotation { get; set; }
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
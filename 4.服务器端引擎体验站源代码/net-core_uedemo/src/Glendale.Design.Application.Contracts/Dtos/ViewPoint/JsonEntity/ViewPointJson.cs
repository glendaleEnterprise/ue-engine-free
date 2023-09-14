using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.ViewPoint.JsonEntity
{

    /// <summary>
    /// 视点上传后json转换的实体
    /// </summary>

    public class Rootobject
    {
        public Xml xml { get; set; }
        public Exchange exchange { get; set; }
    }

    public class Xml
    {
        public string version { get; set; }
        public string encoding { get; set; }
    }

    public class Exchange
    {
        public string xmlnsxsi { get; set; }
        public string xsinoNamespaceSchemaLocation { get; set; }
        public string units { get; set; }
        public string filename { get; set; }
        public string filepath { get; set; }
        public Viewpoints viewpoints { get; set; }
    }

    public class Viewpoints
    {
        public View1[] view { get; set; }
        public Viewfolder viewfolder { get; set; }
    }

    public class Viewfolder
    {
        public string name { get; set; }
        public string guid { get; set; }
        public View view { get; set; }
    }

    public class View
    {
        public string name { get; set; }
        public string guid { get; set; }
        public Viewpoint viewpoint { get; set; }
        public Clipplaneset clipplaneset { get; set; }
    }

    public class Viewpoint
    {
        public string tool { get; set; }
        public string render { get; set; }
        public string lighting { get; set; }
        public string focal { get; set; }
        public string linear { get; set; }
        public string angular { get; set; }
        public Camera camera { get; set; }
        public Viewer viewer { get; set; }
        public Up up { get; set; }
    }

    public class Camera
    {
        public string projection { get; set; }
        public string near { get; set; }
        public string far { get; set; }
        public string aspect { get; set; }
        public string height { get; set; }
        public Position position { get; set; }
        public Rotation rotation { get; set; }
    }

    public class Position
    {
        public Pos3f pos3f { get; set; }
    }

    public class Pos3f
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Rotation
    {
        public Quaternion quaternion { get; set; }
    }

    public class Quaternion
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
    }

    public class Viewer
    {
        public string radius { get; set; }
        public string height { get; set; }
        public string actual_height { get; set; }
        public string eye_height { get; set; }
        public string avatar { get; set; }
        public string camera_mode { get; set; }
        public string first_to_third_angle { get; set; }
        public string first_to_third_distance { get; set; }
        public string first_to_third_param { get; set; }
        public string first_to_third_correction { get; set; }
        public string collision_detection { get; set; }
        public string auto_crouch { get; set; }
        public string gravity { get; set; }
        public string gravity_value { get; set; }
        public string terminal_velocity { get; set; }
    }

    public class Up
    {
        public Vec3f vec3f { get; set; }
    }

    public class Vec3f
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Clipplaneset
    {
        public string linked { get; set; }
        public string current { get; set; }
        public string mode { get; set; }
        public string enabled { get; set; }
        public Range range { get; set; }
        public Clipplanes clipplanes { get; set; }
        public Box box { get; set; }
        public BoxRotation boxrotation { get; set; }
    }

    public class Range
    {
        public Box3f box3f { get; set; }
    }

    public class Box3f
    {
        public Min min { get; set; }
        public Max max { get; set; }
    }

    public class Min
    {
        public Pos3f1 pos3f { get; set; }
    }

    public class Pos3f1
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Max
    {
        public Pos3f2 pos3f { get; set; }
    }

    public class Pos3f2
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Clipplanes
    {
        public Clipplane[] clipplane { get; set; }
    }

    public class Clipplane
    {
        public string state { get; set; }
        public string distance { get; set; }
        public string alignment { get; set; }
        public Plane plane { get; set; }
    }

    public class Plane
    {
        public string distance { get; set; }
        public Vec3f1 vec3f { get; set; }
    }

    public class Vec3f1
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Box
    {
        public Box3f1 box3f { get; set; }
    }

    public class Box3f1
    {
        public Min1 min { get; set; }
        public Max1 max { get; set; }
    }

    public class Min1
    {
        public Pos3f3 pos3f { get; set; }
    }

    public class Pos3f3
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Max1
    {
        public Pos3f4 pos3f { get; set; }
    }

    public class Pos3f4
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class BoxRotation
    {
        public Rotation1 rotation { get; set; }
    }

    public class Rotation1
    {
        public Quaternion1 quaternion { get; set; }
    }

    public class Quaternion1
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
    }

    public class View1
    {
        public string name { get; set; }
        public string guid { get; set; }
        public Viewpoint1 viewpoint { get; set; }
        public Clipplaneset1 clipplaneset { get; set; }
        public Comments comments { get; set; }
        public Redlines redlines { get; set; }
    }

    public class Viewpoint1
    {
        public string tool { get; set; }
        public string render { get; set; }
        public string lighting { get; set; }
        public string focal { get; set; }
        public string linear { get; set; }
        public string angular { get; set; }
        public Camera1 camera { get; set; }
        public Viewer1 viewer { get; set; }
        public Up1 up { get; set; }
    }

    public class Camera1
    {
        public string projection { get; set; }
        public string near { get; set; }
        public string far { get; set; }
        public string aspect { get; set; }
        public string height { get; set; }
        public Position1 position { get; set; }
        public Rotation2 rotation { get; set; }
    }

    public class Position1
    {
        public Pos3f5 pos3f { get; set; }
    }

    public class Pos3f5
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Rotation2
    {
        public Quaternion2 quaternion { get; set; }
    }

    public class Quaternion2
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
    }

    public class Viewer1
    {
        public string radius { get; set; }
        public string height { get; set; }
        public string actual_height { get; set; }
        public string eye_height { get; set; }
        public string avatar { get; set; }
        public string camera_mode { get; set; }
        public string first_to_third_angle { get; set; }
        public string first_to_third_distance { get; set; }
        public string first_to_third_param { get; set; }
        public string first_to_third_correction { get; set; }
        public string collision_detection { get; set; }
        public string auto_crouch { get; set; }
        public string gravity { get; set; }
        public string gravity_value { get; set; }
        public string terminal_velocity { get; set; }
    }

    public class Up1
    {
        public Vec3f2 vec3f { get; set; }
    }

    public class Vec3f2
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Clipplaneset1
    {
        public string linked { get; set; }
        public string current { get; set; }
        public string mode { get; set; }
        public string enabled { get; set; }
        public Range1 range { get; set; }
        public Clipplanes1 clipplanes { get; set; }
        public Box1 box { get; set; }
        public BoxRotation1 boxrotation { get; set; }
    }

    public class Range1
    {
        public Box3f2 box3f { get; set; }
    }

    public class Box3f2
    {
        public Min2 min { get; set; }
        public Max2 max { get; set; }
    }

    public class Min2
    {
        public Pos3f6 pos3f { get; set; }
    }

    public class Pos3f6
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Max2
    {
        public Pos3f7 pos3f { get; set; }
    }

    public class Pos3f7
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Clipplanes1
    {
        public Clipplane1[] clipplane { get; set; }
    }

    public class Clipplane1
    {
        public string state { get; set; }
        public string distance { get; set; }
        public string alignment { get; set; }
        public Plane1 plane { get; set; }
    }

    public class Plane1
    {
        public string distance { get; set; }
        public Vec3f3 vec3f { get; set; }
    }

    public class Vec3f3
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Box1
    {
        public Box3f3 box3f { get; set; }
    }

    public class Box3f3
    {
        public Min3 min { get; set; }
        public Max3 max { get; set; }
    }

    public class Min3
    {
        public Pos3f8 pos3f { get; set; }
    }

    public class Pos3f8
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Max3
    {
        public Pos3f9 pos3f { get; set; }
    }

    public class Pos3f9
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class BoxRotation1
    {
        public Rotation3 rotation { get; set; }
    }

    public class Rotation3
    {
        public Quaternion3 quaternion { get; set; }
    }

    public class Quaternion3
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public string d { get; set; }
    }

    public class Comments
    {
        public Comment comment { get; set; }
    }

    public class Comment
    {
        public string id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public string body { get; set; }
        public Createddate createddate { get; set; }
    }

    public class Createddate
    {
        public Date date { get; set; }
    }

    public class Date
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
        public string second { get; set; }
    }

    public class Redlines
    {
        public Rltag rltag { get; set; }
        public Rlellipse rlellipse { get; set; }
    }

    public class Rltag
    {
        public string thickness { get; set; }
        public string pattern { get; set; }
        public string id { get; set; }
        public string commentid { get; set; }
        public Colour colour { get; set; }
        public Pos1 pos1 { get; set; }
        public Pos2 pos2 { get; set; }
        public Pos3d pos3d { get; set; }
        public Bounds bounds { get; set; }
    }

    public class Colour
    {
        public string red { get; set; }
        public string green { get; set; }
        public string blue { get; set; }
    }

    public class Pos1
    {
        public Pos2f pos2f { get; set; }
    }

    public class Pos2f
    {
        public string x { get; set; }
        public string y { get; set; }
    }

    public class Pos2
    {
        public Pos2f1 pos2f { get; set; }
    }

    public class Pos2f1
    {
        public string x { get; set; }
        public string y { get; set; }
    }

    public class Pos3d
    {
        public Pos3f10 pos3f { get; set; }
    }

    public class Pos3f10
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Bounds
    {
        public Box3f4 box3f { get; set; }
    }

    public class Box3f4
    {
        public Min4 min { get; set; }
        public Max4 max { get; set; }
    }

    public class Min4
    {
        public Pos3f11 pos3f { get; set; }
    }

    public class Pos3f11
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Max4
    {
        public Pos3f12 pos3f { get; set; }
    }

    public class Pos3f12
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class Rlellipse
    {
        public string thickness { get; set; }
        public string pattern { get; set; }
        public Colour1 colour { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
    }

    public class Colour1
    {
        public string red { get; set; }
        public string green { get; set; }
        public string blue { get; set; }
    }

    public class Start
    {
        public Pos2f2 pos2f { get; set; }
    }

    public class Pos2f2
    {
        public string x { get; set; }
        public string y { get; set; }
    }

    public class End
    {
        public Pos2f3 pos2f { get; set; }
    }

    public class Pos2f3
    {
        public string x { get; set; }
        public string y { get; set; }
    }

}

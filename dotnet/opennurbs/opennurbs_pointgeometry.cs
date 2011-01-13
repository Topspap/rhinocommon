using System;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace Rhino.Geometry
{
  public class Point : GeometryBase
  {
    internal Point(IntPtr native_pointer, Rhino.DocObjects.RhinoObject parent_object, Rhino.DocObjects.ObjRef obj_ref)
      : base(native_pointer, parent_object, obj_ref)
    { }

    public Point(Point3d location)
    {
      IntPtr ptr = UnsafeNativeMethods.ON_Point_New(location);
      ConstructNonConstObject(ptr);
    }

    public Point3d Location
    {
      get
      {
        Point3d pt = new Point3d();
        IntPtr ptr = ConstPointer();
        UnsafeNativeMethods.ON_Point_GetSetPoint(ptr, false, ref pt);
        return pt;
      }
      set
      {
        IntPtr ptr = NonConstPointer();
        UnsafeNativeMethods.ON_Point_GetSetPoint(ptr, true, ref value);
      }
    }

    internal override GeometryBase DuplicateShallowHelper()
    {
      return new Point(IntPtr.Zero, null, null);
    }
  }
}
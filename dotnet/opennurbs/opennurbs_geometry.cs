using System;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;

namespace Rhino.Geometry
{
  public class GeometryBase : Runtime.CommonObject
  {
    #region constructors / wrapped pointer manipulation
    Geometry.GeometryBase m_shallow_parent;

    protected GeometryBase() { }

    internal override IntPtr _InternalDuplicate(out bool applymempressure)
    {
      applymempressure = true;
      IntPtr pConstPointer = ConstPointer();
      IntPtr rc = UnsafeNativeMethods.ON_Object_Duplicate(pConstPointer);
      return rc;
    }

    internal override IntPtr _InternalGetConstPointer()
    {
      if (null != m_shallow_parent)
        return m_shallow_parent.ConstPointer();

      Rhino.DocObjects.ObjRef obj_ref = this.m__parent as Rhino.DocObjects.ObjRef;
      if (null != obj_ref)
        return obj_ref.GetGeometryConstPointer(this);

      uint serial_number = 0;
      Rhino.DocObjects.RhinoObject parent_object = ParentRhinoObject();
      if (null != parent_object)
        serial_number = parent_object.m_rhinoobject_serial_number;
      ComponentIndex ci = new ComponentIndex();
      return UnsafeNativeMethods.CRhinoObject_Geometry(serial_number, ci);
    }

    internal override object _GetConstObjectParent()
    {
      if (!IsDocumentControlled)
        return null;
      if (null != m_shallow_parent)
        return m_shallow_parent;
      return base._GetConstObjectParent();
    }


    protected override void OnSwitchToNonConst()
    {
      m_shallow_parent = null;
      base.OnSwitchToNonConst();
    }

    /// <summary>
    /// If true this object may not be modified. Any properties or functions that attempt
    /// to modify this object when it is set to "IsReadOnly" will throw a NotSupportedException
    /// </summary>
    public sealed override bool IsDocumentControlled
    {
      get
      {
        if (null != m_shallow_parent)
          return m_shallow_parent.IsDocumentControlled;
        return base.IsDocumentControlled;
      }
    }


    public GeometryBase DuplicateShallow()
    {
      GeometryBase rc = DuplicateShallowHelper();
      if (null != rc)
        rc.m_shallow_parent = this;
      return rc;
    }
    internal virtual GeometryBase DuplicateShallowHelper()
    {
      return null;
    }

    public virtual GeometryBase Duplicate()
    {
      IntPtr ptr = ConstPointer();
      IntPtr pNewGeometry = UnsafeNativeMethods.ON_Object_Duplicate(ptr);
      return CreateGeometryHelper(pNewGeometry, null);
    }


    internal GeometryBase(IntPtr ptr, Rhino.DocObjects.RhinoObject parent_object, Rhino.DocObjects.ObjRef obj_ref)
    {
      object parent = parent_object;
      if (parent == null)
        parent = obj_ref;
      if (null == parent)
        ConstructNonConstObject(ptr);
      else
        ConstructConstObject(parent, -1);
    }
    internal GeometryBase(IntPtr ptr, object parent, int subobject_index)
    {
      if (subobject_index >= 0 && parent == null)
      {
        throw new ArgumentException();
      }

      if (null == parent)
        ConstructNonConstObject(ptr);
      else
        ConstructConstObject(parent, subobject_index);
    }

    #region Object type codes
    internal const int idxON_Geometry = 0;
    internal const int idxON_Curve = 1;
    internal const int idxON_NurbsCurve = 2;
    internal const int idxON_PolyCurve = 3;
    internal const int idxON_PolylineCurve = 4;
    internal const int idxON_ArcCurve = 5;
    internal const int idxON_LineCurve = 6;
    const int idxON_Mesh = 7;
    const int idxON_Point = 8;
    const int idxON_TextDot = 9;
    const int idxON_Surface = 10;
    const int idxON_Brep = 11;
    const int idxON_NurbsSurface = 12;
    const int idxON_RevSurface = 13;
    const int idxON_PlaneSurface = 14;
    const int idxON_ClippingPlaneSurface = 15;
    const int idxON_Annotation2 = 16;
    const int idxON_Hatch = 17;
    const int idxON_TextEntity2 = 18;
    const int idxON_SumSurface = 19;
    const int idxON_BrepFace = 20;
    const int idxON_BrepEdge = 21;
    const int idxON_InstanceDefinition = 22;
    const int idxON_InstanceReference = 23;
    const int idxON_Extrusion = 24;
    const int idxON_LinearDimension2 = 25;
    const int idxON_PointCloud = 26;
    const int idxON_DetailView = 27;
    const int idxON_AngularDimension2 = 28;
    const int idxON_RadialDimension2 = 29;
    const int idxON_Leader = 30;
    const int idxON_OrdinateDimension2 = 31;
    const int idxON_Light = 32;
    #endregion

    internal static GeometryBase CreateGeometryHelper(IntPtr pGeometry, object parent)
    {
      return CreateGeometryHelper(pGeometry, parent, -1);
    }

    internal static GeometryBase CreateGeometryHelper(IntPtr pGeometry, object parent, int subobject_index)
    {
      if (IntPtr.Zero == pGeometry)
        return null;

      int type = UnsafeNativeMethods.ON_Geometry_GetGeometryType(pGeometry);
      if (type < 0)
        return null;
      GeometryBase rc = null;
      RhinoObject parent_object = parent as RhinoObject;
      ObjRef source_objref = parent as ObjRef;
      switch (type)
      {
        case idxON_Curve: //1
          rc = new Curve(pGeometry, parent, subobject_index);
          break;
        case idxON_NurbsCurve: //2
          rc = new NurbsCurve(pGeometry, parent, subobject_index);
          break;
        case idxON_PolyCurve: // 3
          rc = new PolyCurve(pGeometry, parent, subobject_index);
          break;
        case idxON_PolylineCurve: //4
          rc = new PolylineCurve(pGeometry, parent, subobject_index);
          break;
        case idxON_ArcCurve: //5
          rc = new ArcCurve(pGeometry, parent, subobject_index);
          break;
        case idxON_LineCurve: //6
          rc = new LineCurve(pGeometry, parent, subobject_index);
          break;
        case idxON_Mesh: //7
          rc = new Mesh(pGeometry, parent_object, source_objref);
          break;
        case idxON_Point: //8
          rc = new Point(pGeometry, parent_object, source_objref);
          break;
        case idxON_TextDot: //9
          rc = new TextDot(pGeometry, parent_object, source_objref);
          break;
        case idxON_Surface: //10
          rc = new Surface(pGeometry, parent_object, source_objref);
          break;
        case idxON_Brep: //11
          rc = new Brep(pGeometry, parent_object, source_objref);
          break;
        case idxON_NurbsSurface: //12
          rc = new NurbsSurface(pGeometry, parent_object, source_objref);
          break;
        case idxON_RevSurface: //13
          rc = new RevSurface(pGeometry, parent_object, source_objref);
          break;
        case idxON_PlaneSurface: //14
          rc = new PlaneSurface(pGeometry, parent_object, source_objref);
          break;
        case idxON_ClippingPlaneSurface: //15
          rc = new ClippingPlaneSurface(pGeometry, parent_object, source_objref);
          break;
        case idxON_Annotation2: // 16
          rc = new AnnotationBase(pGeometry, parent_object, source_objref);
          break;
        case idxON_Hatch: // 17
          rc = new Hatch(pGeometry, parent_object, source_objref);
          break;
        case idxON_TextEntity2: //18
          rc = new TextEntity(pGeometry, parent_object, source_objref);
          break;
        case idxON_SumSurface: //19
          rc = new SumSurface(pGeometry, parent_object, source_objref);
          break;
        case idxON_BrepFace: //20
          {
            int faceindex = -1;
            IntPtr pBrep = UnsafeNativeMethods.ON_BrepSubItem_Brep(pGeometry, ref faceindex);
            if (pBrep != IntPtr.Zero && faceindex >= 0)
            {
              Brep b = new Brep(pBrep, parent_object, source_objref);
              rc = b.Faces[faceindex];
            }
          }
          break;
        case idxON_BrepEdge: // 21
          {
            int edgeindex = -1;
            IntPtr pBrep = UnsafeNativeMethods.ON_BrepSubItem_Brep(pGeometry, ref edgeindex);
            if (pBrep != IntPtr.Zero && edgeindex >= 0)
            {
              Brep b = new Brep(pBrep, parent_object, source_objref);
              rc = b.Edges[edgeindex];
            }
          }
          break;
        case idxON_InstanceDefinition: // 22
          rc = new InstanceDefinitionGeometry(pGeometry, parent_object, source_objref);
          break;
        case idxON_InstanceReference: // 23
          rc = new InstanceReferenceGeometry(pGeometry, parent_object, source_objref);
          break;
#if USING_V5_SDK
        case idxON_Extrusion: //24
          rc = new Extrusion(pGeometry, parent_object, source_objref);
          break;
#endif
        case idxON_LinearDimension2: //25
          rc = new LinearDimension(pGeometry, parent_object, source_objref);
          break;
        case idxON_PointCloud: // 26
          rc = new PointCloud(pGeometry, parent_object, source_objref);
          break;
        case idxON_DetailView: // 27
          rc = new DetailView(pGeometry, parent_object, source_objref);
          break;
        case idxON_AngularDimension2: // 28
          rc = new AngularDimension(pGeometry, parent_object, source_objref);
          break;
        case idxON_RadialDimension2: // 29
          rc = new RadialDimension(pGeometry, parent_object, source_objref);
          break;
        case idxON_Leader: // 30
          rc = new Leader(pGeometry, parent_object, source_objref);
          break;
        case idxON_OrdinateDimension2: // 31
          rc = new OrdinateDimension(pGeometry, parent_object, source_objref);
          break;
        case idxON_Light: //32
          rc = new Light(pGeometry, parent_object, source_objref);
          break;
        default:
          rc = new GeometryBase(pGeometry, parent, subobject_index);
          break;
      }

      return rc;
    }

    #endregion

    // [skipping from ON_Object]
    //  BOOL IsValid( ON_TextLog* text_log = NULL ) const;

    /// <summary>
    /// Useful for switch statements that need to differentiate between
    /// basic object types like points, curves, surfaces, and so on.
    /// </summary>
    [CLSCompliant(false)]
    public ObjectType ObjectType
    {
      get
      {
        IntPtr ptr = ConstPointer();
        uint rc = UnsafeNativeMethods.ON_Object_ObjectType(ptr);
        return (ObjectType)rc;
      }
    }

    //[skipping]
    // BOOL Rotate( double, double, ON_3dVector, ON_3dPoint);

    #region Transforms
    /// <summary>
    /// Transforms the geometry. If the input Transform has a SimilarityType of
    /// OrientationReversing, you may want to consider flipping the transformed
    /// geometry after calling this function when it makes sense. For example,
    /// you may want to call Flip() on a Brep after transforming it.
    /// </summary>
    /// <param name="xform">
    /// Transformation to apply to geometry.
    /// </param>
    /// <returns></returns>
    public bool Transform(Transform xform)
    {
      IntPtr ptr = NonConstPointer();
      return UnsafeNativeMethods.ON_Geometry_Transform(ptr, ref xform);
    }

    /// <summary>Translates the object along the specified vector.</summary>
    /// <param name="translationVector"></param>
    /// <returns>true if geometry successfully translated</returns>
    public bool Translate(Vector3d translationVector)
    {
      IntPtr ptr = NonConstPointer();
      return UnsafeNativeMethods.ON_Geometry_Translate(ptr, translationVector);
    }

    /// <summary>Translates the object along the specified vector.</summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>true if geometry successfully translated</returns>
    public bool Translate(double x, double y, double z)
    {
      Vector3d t = new Vector3d(x, y, z);
      return Translate(t);
    }

    /// <summary>
    /// Scales the object by the specified factor. The scale is centered at the origin.
    /// </summary>
    /// <param name="scaleFactor"></param>
    /// <returns>true if geometry successfully scaled</returns>
    public bool Scale(double scaleFactor)
    {
      IntPtr ptr = NonConstPointer();
      return UnsafeNativeMethods.ON_Geometry_Scale(ptr, scaleFactor);
    }

    /// <summary>
    /// Rotates the object about the specified axis. A positive rotation 
    /// angle results in a counter-clockwise rotation about the axis (right hand rule).
    /// </summary>
    /// <param name="angleRadians">angle of rotation in radians</param>
    /// <param name="rotationAxis">direction of the axis of rotation</param>
    /// <param name="rotationCenter">point on the axis of rotation</param>
    /// <returns>true if geometry successfully rotated</returns>
    public bool Rotate(double angleRadians, Vector3d rotationAxis, Point3d rotationCenter)
    {
      IntPtr ptr = NonConstPointer();
      return UnsafeNativeMethods.ON_Geometry_Rotate(ptr, angleRadians, rotationAxis, rotationCenter);
    }
    #endregion

    /// <summary>
    /// Boundingbox solver. Gets the world axis aligned boundingbox for the geometry.
    /// </summary>
    /// <param name="accurate">If True, a physically accurate boundingbox will be computed. 
    /// If not, a boundingbox estimate will be computed. For some geometry types there is no 
    /// difference between the estimate and the accurate boundingbox. Estimated boundingboxes 
    /// can be computed much (much) faster than accurate (or "tight") bounding boxes. 
    /// Estimated bounding boxes are always similar to or larger than accurate bounding boxes.</param>
    /// <returns>The boundingbox of the geometry in world coordinates or BoundingBox.Empty 
    /// if not bounding box could be found.</returns>
    public BoundingBox GetBoundingBox(bool accurate)
    {
      Rhino.DocObjects.RhinoObject parent_object = ParentRhinoObject();
      if (accurate)
      {
        BoundingBox bbox = new BoundingBox();
        Transform xf = new Transform();
        if (null != parent_object)
        {
          IntPtr pParentObject = parent_object.ConstPointer();
          if (UnsafeNativeMethods.CRhinoObject_GetTightBoundingBox(pParentObject, ref bbox, ref xf, false))
            return bbox;
        }
        IntPtr ptr = ConstPointer();
        if (UnsafeNativeMethods.ON_Geometry_GetTightBoundingBox(ptr, ref bbox, ref xf, false))
          return bbox;
        return BoundingBox.Empty;
      }
      else
      {
        BoundingBox rc = new BoundingBox();
        if (null != parent_object)
        {
          IntPtr pParentObject = parent_object.ConstPointer();
          if (UnsafeNativeMethods.CRhinoObject_BoundingBox(pParentObject, ref rc))
            return rc;
        }
        IntPtr ptr = ConstPointer();
        UnsafeNativeMethods.ON_Geometry_BoundingBox(ptr, ref rc);
        return rc;
      }
    }
    /// <summary>
    /// Aligned Boundingbox solver. Gets the world axis aligned boundingbox for the transformed geometry.
    /// </summary>
    /// <param name="xform">Transformation to apply to object prior to the BoundingBox computation. 
    /// The geometry itself is not modified.</param>
    /// <returns>The accurate boundingbox of the transformed geometry in world coordinates 
    /// or BoundingBox.Empty if not bounding box could be found.</returns>
    public BoundingBox GetBoundingBox(Transform xform)
    {
      BoundingBox bbox = BoundingBox.Empty;

      // In cases like breps and curves, the CRhinoBrepObject and CRhinoCurveObject
      // can compute a better tight bounding box
      Rhino.DocObjects.RhinoObject parent_object = ParentRhinoObject();
      if (parent_object != null)
      {
        IntPtr pParent = parent_object.ConstPointer();
        if (UnsafeNativeMethods.CRhinoObject_GetTightBoundingBox(pParent, ref bbox, ref xform, true))
          return bbox;
      }
      IntPtr ptr = ConstPointer();
      if (UnsafeNativeMethods.ON_Geometry_GetTightBoundingBox(ptr, ref bbox, ref xform, true))
        return bbox;
      return BoundingBox.Empty;
    }
    /// <summary>
    /// Aligned Boundingbox solver. Gets the plane aligned boundingbox.
    /// </summary>
    /// <param name="plane">Orientation plane for BoundingBox.</param>
    /// <returns>A BoundingBox in plane coordinates.</returns>
    public BoundingBox GetBoundingBox(Plane plane)
    {
      if (!plane.IsValid) { return BoundingBox.Unset; }

      Transform xform = Geometry.Transform.ChangeBasis(Plane.WorldXY, plane);
      BoundingBox rc = GetBoundingBox(xform);
      return rc;
    }
    /// <summary>
    /// Aligned Boundingbox solver. Gets the plane aligned boundingbox.
    /// </summary>
    /// <param name="plane">Orientation plane for BoundingBox.</param>
    /// <param name="worldBox">Aligned box in World coordinates.</param>
    /// <returns>A BoundingBox in plane coordinates.</returns>
    public BoundingBox GetBoundingBox(Plane plane, out Box worldBox)
    {
      worldBox = Box.Unset;

      if (!plane.IsValid) { return BoundingBox.Unset; }

      Transform xform = Geometry.Transform.ChangeBasis(Plane.WorldXY, plane);
      BoundingBox rc = GetBoundingBox(xform);

      //Transform unxform;
      //xform.TryGetInverse(out unxform);

      //worldBox = new Box(rc);
      //worldBox.Transform(unxform);

      worldBox = new Box(plane, rc);
      return rc;
    }


    ///// <summary>
    ///// The dimension is typically three. For parameter space trimming curves the
    ///// dimension is two. In rare cases the dimension can be one or greater than three.
    ///// </summary>
    //public int Dimension
    //{
    //  get
    //  {
    //    IntPtr ptr = ConstPointer();
    //    return UnsafeNativeMethods.ON_Geometry_Dimension(ptr);
    //  }
    //}



    // [skipping]
    // BOOL GetBBox

    // [skipping]
    // void ClearBoundingBox();



    const int idxIsDeformable = 0;
    const int idxMakeDeformable = 1;
    internal const int idxIsMorphable = 2;
    const int idxHasBrepForm = 3;

    /// <summary>
    /// True if object can be accurately modified with "squishy" transformations like
    /// projections, shears, and non-uniform scaling.
    /// </summary>
    public bool IsDeformable
    {
      get
      {
        IntPtr ptr = ConstPointer();
        return UnsafeNativeMethods.ON_Geometry_GetBool(ptr, idxIsDeformable);
      }
    }

    /// <summary>
    /// If possible, converts the object into a form that can be accurately modified
    /// with "squishy" transformations like projections, shears, an non-uniform scaling.
    /// </summary>
    /// <returns>
    /// False if object cannot be converted to a deformable object. True if object was
    /// already deformable or was converted into a deformable object.
    /// </returns>
    public bool MakeDeformable()
    {
      IntPtr ptr = NonConstPointer();
      return UnsafeNativeMethods.ON_Geometry_GetBool(ptr, idxMakeDeformable);
    }

    // [skipping] BOOL SwapCoordinates( int i, int j );

    // Not exposed here
    // virtual bool Morph( const ON_SpaceMorph& morph );
    // virtual bool IsMorphable() const;
    // Moved to SpaceMorph class

    // Not exposed here
    // bool HasBrepForm() const;
    // ON_Brep* BrepForm( ON_Brep* brep = NULL ) const;
    // Implemented in static Brep.TryConvertBrep function

    /// <summary>
    /// If this piece of geometry is a component in something larger, like a BrepEdge
    /// in a Brep, then this function returns the component index.
    /// </summary>
    /// <returns>
    /// This object's component index.  If this object is not a sub-piece of a larger
    /// geometric entity, then the returned index has 
    /// m_type = ComponentIndex.InvalidType
    /// and m_index = -1.
    /// </returns>
    public ComponentIndex ComponentIndex()
    {
      ComponentIndex ci = new ComponentIndex();
      IntPtr ptr = ConstPointer();
      UnsafeNativeMethods.ON_Geometry_ComponentIndex(ptr, ref ci);
      return ci;
    }

    // [skipping]
    // bool EvaluatePoint( const class ON_ObjRef& objref, ON_3dPoint& P ) const;

    #region user strings
    /// <summary>
    /// Attach a user string (key,value combination) to this geometry
    /// </summary>
    /// <param name="key">id used to retrieve this string</param>
    /// <param name="value">string associated with key</param>
    /// <returns>true on success</returns>
    public bool SetUserString(string key, string value)
    {
      //const lie
      IntPtr pThis = ConstPointer();
      bool rc = UnsafeNativeMethods.ON_Object_SetUserString(pThis, key, value);
      return rc;
    }
    /// <summary>
    /// Get user string from this geometry
    /// </summary>
    /// <param name="key">id used to retrieve the string</param>
    /// <returns>string associated with the key if successful. null if no key was found</returns>
    public string GetUserString(string key)
    {
      IntPtr pThis = ConstPointer();
      IntPtr pValue = UnsafeNativeMethods.ON_Object_GetUserString(pThis, key);
      if (IntPtr.Zero == pValue)
        return null;
      return Marshal.PtrToStringUni(pValue);
    }

    public int UserStringCount
    {
      get
      {
        IntPtr pThis = ConstPointer();
        int rc = UnsafeNativeMethods.ON_Object_UserStringCount(pThis);
        return rc;
      }
    }

    /// <summary>
    /// Get all (key, value) user strings attached to this geometry
    /// </summary>
    /// <returns></returns>
    public System.Collections.Specialized.NameValueCollection GetUserStrings()
    {
      System.Collections.Specialized.NameValueCollection rc = new System.Collections.Specialized.NameValueCollection();
      IntPtr pThis = ConstPointer();
      int count = 0;
      IntPtr pUserStrings = UnsafeNativeMethods.ON_Object_GetUserStrings(pThis, ref count);

      for (int i = 0; i < count; i++)
      {
        IntPtr pKey = UnsafeNativeMethods.ON_UserStringList_KeyValue(pUserStrings, i, true);
        IntPtr pValue = UnsafeNativeMethods.ON_UserStringList_KeyValue(pUserStrings, i, false);
        if (IntPtr.Zero != pKey && IntPtr.Zero != pValue)
        {
          string key = Marshal.PtrToStringUni(pKey);
          string value = Marshal.PtrToStringUni(pValue);
          rc.Add(key, value);
        }
      }

      if (IntPtr.Zero != pUserStrings)
        UnsafeNativeMethods.ON_UserStringList_Delete(pUserStrings);

      return rc;
    }
    #endregion

  }
}
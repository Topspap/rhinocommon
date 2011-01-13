using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rhino.DocObjects
{
  public class Linetype : Rhino.Runtime.CommonObject
  {
    #region members
    // Represents both a CRhinoLinetype and an ON_Linetype. When m_ptr is
    // null, the object uses m_doc and m_id to look up the const
    // CRhinoLinetype in the linetype table.
    Rhino.RhinoDoc m_doc;
    Guid m_id=Guid.Empty;
    #endregion

    #region constructors
    public Linetype()
    {
      // Creates a new non-document control ON_Linetype
      IntPtr pLinetype = UnsafeNativeMethods.ON_Linetype_New();
      base.ConstructNonConstObject(pLinetype);
    }

    internal Linetype(int index, Rhino.RhinoDoc doc)
    {
      m_id = UnsafeNativeMethods.CRhinoLinetypeTable_GetLinetypeId(doc.m_docId, index);
      m_doc = doc;
      this.m__parent = m_doc;
    }
    #endregion

    public bool CommitChanges()
    {
      if (m_id == Guid.Empty || IsDocumentControlled)
        return false;
      IntPtr pThis = NonConstPointer();
      return UnsafeNativeMethods.CRhinoLinetypeTable_CommitChanges(m_doc.m_docId, pThis, m_id);
    }

    internal override IntPtr _InternalGetConstPointer()
    {
      if (m_doc != null)
        return UnsafeNativeMethods.CRhinoLinetypeTable_GetLinetypePointer2(m_doc.m_docId, m_id);
      return IntPtr.Zero;
    }

    internal override IntPtr _InternalDuplicate(out bool applymempressure)
    {
      applymempressure = false;
      IntPtr pConstPointer = ConstPointer();
      return UnsafeNativeMethods.ON_Object_Duplicate(pConstPointer);
    }

    #region properties
    /// <summary>The name of this linetype</summary>
    public string Name
    {
      get
      {
        using (Rhino.Runtime.StringHolder sh = new Rhino.Runtime.StringHolder())
        {
          IntPtr pLinetype = ConstPointer();
          IntPtr pString = sh.NonConstPointer();
          UnsafeNativeMethods.ON_Linetype_GetLinetypeName(pLinetype, pString);
          return sh.ToString();
        }
      }
      set
      {
        IntPtr pThis = NonConstPointer();
        UnsafeNativeMethods.ON_Linetype_SetLinetypeName(pThis, value);
      }
    }

    /// <summary>The index of this linetype.</summary>
    public int LinetypeIndex
    {
      get { return GetInt(idxLinetypeIndex); }
      set { SetInt(idxLinetypeIndex, value); }
    }

    /// <summary>Total length of one repeat of the pattern</summary>
    public double PatternLength
    {
      get
      {
        IntPtr pConstThis = ConstPointer();
        return UnsafeNativeMethods.ON_Linetype_PatternLength(pConstThis);
      }
    }

    /// <summary>Number of segments in the pattern</summary>
    public int SegmentCount
    {
      get { return GetInt(idxSegmentCount); }
    }

    /// <summary>
    /// Gets the ID of this linetype object.
    /// </summary>
    public Guid Id
    {
      get
      {
        IntPtr pLinetype = ConstPointer();
        return UnsafeNativeMethods.ON_Linetype_GetGuid(pLinetype);
      }
      set
      {
        IntPtr ptr = NonConstPointer();
        UnsafeNativeMethods.ON_Linetype_SetGuid(ptr, value);
      }
    }

    /// <summary>
    /// Gets a value indicating whether this linetype has been deleted and is 
    /// currently in the Undo buffer.
    /// </summary>
    public bool IsDeleted
    {
      get
      {
        if (null == m_doc)
          return false;
        int index = LinetypeIndex;
        return UnsafeNativeMethods.CRhinoLinetype_IsDeleted(m_doc.m_docId, index);
      }
    }

    /// <summary>
    /// Gets a value indicting whether this linetype is a referenced linetype. 
    /// Referenced linetypes are part of referenced documents.
    /// </summary>
    public bool IsReference
    {
      get
      {
        if (null == m_doc)
          return false;
        int index = LinetypeIndex;
        return UnsafeNativeMethods.CRhinoLinetype_IsReference(m_doc.m_docId, index);
      }
    }

    /// <summary>
    /// true if this linetype has been modified by LinetypeTable.ModifyLinetype()
    /// and the modifications can be undone.
    /// </summary>
    public bool IsModified
    {
      get
      {
        if (null == m_doc)
          return false;
        int index = LinetypeIndex;
        return UnsafeNativeMethods.CRhinoLinetype_IsModified(m_doc.m_docId, index);
      }
    }

    const int idxLinetypeIndex = 0;
    const int idxSegmentCount = 1;
    int GetInt(int which)
    {
      IntPtr pConstLinetype = ConstPointer();
      return UnsafeNativeMethods.ON_Linetype_GetInt(pConstLinetype, which);
    }
    void SetInt(int which, int val)
    {
      IntPtr ptr = NonConstPointer();
      UnsafeNativeMethods.ON_Linetype_SetInt(ptr, which, val);
    }
    #endregion

    #region methods
    /// <summary>
    /// Set linetype to default settings
    /// </summary>
    public void Default()
    {
      IntPtr pThis = NonConstPointer();
      UnsafeNativeMethods.ON_Linetype_Default(pThis);
    }

    /// <summary>Adds a segment to the pattern</summary>
    /// <param name="length"></param>
    /// <param name="isSolid">
    /// If true, the length is interpreted as a line. If false,
    /// the length is interpreted as a space
    /// </param>
    /// <returns>index of the added segment</returns>
    public int AppendSegment(double length, bool isSolid)
    {
      IntPtr pThis = NonConstPointer();
      return UnsafeNativeMethods.ON_Linetype_AppendSegment(pThis, length, isSolid);
    }

    /// <summary>Removes a segment in the linetype</summary>
    /// <param name="index">Zero based index of the segment to remove</param>
    /// <returns>True if the segment index was removed</returns>
    public bool RemoveSegment(int index)
    {
      IntPtr pThis = NonConstPointer();
      return UnsafeNativeMethods.ON_Linetype_RemoveSegment(pThis, index);
    }

    /// <summary>Sets the length and type of the segment at index</summary>
    /// <param name="index">Zero based index of the segment</param>
    /// <param name="length"></param>
    /// <param name="isSolid"></param>
    /// <returns>True on success</returns>
    public bool SetSegment(int index, double length, bool isSolid)
    {
      IntPtr pThis = NonConstPointer();
      return UnsafeNativeMethods.ON_Linetype_SetSegment(pThis, index, length, isSolid);
    }

    /// <summary>
    /// Get segment information at index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <param name="isSolid"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public void GetSegment(int index, out double length, out bool isSolid)
    {
      if (index < 0 || index >= SegmentCount)
        throw new IndexOutOfRangeException();
      IntPtr pConstThis = ConstPointer();
      length = 0;
      isSolid = false;
      UnsafeNativeMethods.ON_Linetype_GetSegment(pConstThis, index, ref length, ref isSolid);
    }
    #endregion
  }
}

namespace Rhino.DocObjects.Tables
{
  public sealed class LinetypeTable
  {
    private RhinoDoc m_doc;
    private LinetypeTable() { }
    internal LinetypeTable(RhinoDoc doc)
    {
      m_doc = doc;
    }

    /// <summary>Document that owns this table</summary>
    public RhinoDoc Document
    {
      get { return m_doc; }
    }

    /// <summary>
    /// Returns number of linetypes in the linetypes table, including deleted linetypes.
    /// </summary>
    public int Count
    {
      get
      {
        return UnsafeNativeMethods.CRhinoLinetypeTable_LinetypeCount(m_doc.m_docId, false);
      }
    }

    /// <summary>
    /// Returns number of linetypes in the linetypes table, excluding deleted linetypes
    /// </summary>
    public int ActiveCount
    {
      get
      {
        return UnsafeNativeMethods.CRhinoLinetypeTable_LinetypeCount(m_doc.m_docId, true);
      }
    }

    /// <summary>
    /// At all times, there is a "current" linetype.  Unless otherwise specified,
    /// new objects are assigned to the current linetype. If the current linetype
    /// source is LinetypeFromLayer the object's layer's linetype is used instead
    /// </summary>
    public int CurrentLinetypeIndex
    {
      get
      {
        return UnsafeNativeMethods.CRhinoLinetypeTable_CurrentLinetypeIndex(m_doc.m_docId);
      }
    }

    /// <summary>
    /// For display in Rhino viewports, the linetypes are scaled by a single scale
    /// factor for all viewports. This is not used for printing, where all linetype
    /// patterns are scaled to print in their defined size 1:1 on the paper.
    /// </summary>
    public double LinetypeScale
    {
      get
      {
        return UnsafeNativeMethods.CRhinoLinetypeTable_GetLinetypeScale(m_doc.m_docId);
      }
      set
      {
        UnsafeNativeMethods.CRhinoLinetypeTable_SetLinetypeScale(m_doc.m_docId, value);
      }
    }

    /// <summary>
    /// Conceptually, the linetype table is an array of linetypes.
    /// The operator[] can be used to get individual linetypes. A linetype is
    /// either active or deleted and this state is reported by Linetype.IsDeleted
    /// </summary>
    /// <param name="index">zero based array index</param>
    /// <returns>
    /// Refererence to the linetype.  If index is out of range, the current
    /// linetype is returned. Note that this reference may become invalid after
    /// AddLinetype() is called.
    /// </returns>
    public DocObjects.Linetype this[int index]
    {
      get
      {
        if (index < 0 || index >= Count)
          index = CurrentLinetypeIndex;
        return new Rhino.DocObjects.Linetype(index, m_doc);
      }
    }

    /// <summary>
    /// Source used by an object to determine its current linetype to be used by new objects.
    /// </summary>
    public Rhino.DocObjects.ObjectLinetypeSource CurrentLinetypeSource
    {
      get
      {
        return (Rhino.DocObjects.ObjectLinetypeSource)UnsafeNativeMethods.CRhinoLinetypeTable_GetCurrentLinetypeSource(m_doc.m_docId);
      }
      set
      {
        UnsafeNativeMethods.CRhinoLinetypeTable_SetCurrentLinetypeSource(m_doc.m_docId, (int)value);
      }
    }

    /// <summary>
    /// At all times, there is a "current" linetype. Unless otherwise specified, new objects
    /// are assigned to the current linetype. The current linetype is never deleted.
    /// </summary>
    /// <param name="linetypeIndex">
    /// Value for new current linetype. 0 &lt;= linetypeIndex &lt; LinetypeTable.Count.
    /// </param>
    /// <param name="quiet">
    /// if true, then no warning message box pops up if the current linetype request can't be satisfied.
    /// </param>
    /// <returns>true if current linetype index successfully set.</returns>
    public bool SetCurrentLinetypeIndex(int linetypeIndex, bool quiet)
    {
      return UnsafeNativeMethods.CRhinoLinetypeTable_SetCurrentLinetypeIndex(m_doc.m_docId, linetypeIndex, quiet);
    }

    /// <summary>
    /// Returns the effective linetype index to be used to find the 
    /// linetype definition to draw an object. If an object's linetype
    /// source is LinetypeFromObject, the linetype index in the object's
    /// attributes is used. If an object's linetype source is LinetypeFromLayer
    /// the linetype index from the object's layer is used.
    /// </summary>
    /// <param name="rhinoObject"></param>
    /// <returns></returns>
    public int LinetypeIndexForObject(Rhino.DocObjects.RhinoObject rhinoObject)
    {
      IntPtr pConstRhinoObject = rhinoObject.ConstPointer();
      return UnsafeNativeMethods.CRhinoLinetypeTable_EffectiveLinetypeIndex(m_doc.m_docId, pConstRhinoObject);
    }

    /// <summary>
    /// At all times, there is a "current" linetype. Unless otherwise specified,
    /// new objects are assigned to the current linetype. The current linetype
    /// is never deleted.
    /// 
    /// Returns reference to the current linetype. Note that this reference may
    /// become invalid after a call to AddLinetype().
    /// </summary>
    public DocObjects.Linetype CurrentLinetype
    {
      get
      {
        return new Rhino.DocObjects.Linetype(CurrentLinetypeIndex, m_doc);
      }
    }

    /// <summary>Finds the linetype with a given name</summary>
    /// <param name="name">search ignores case</param>
    /// <param name="ignoreDeletedLinetypes">If true, deleted linetypes are not checked</param>
    /// <returns>
    /// >=0 index of the linetype with the given name
    /// -1  no linetype has the given name
    /// </returns>
    public int Find(string name, bool ignoreDeletedLinetypes)
    {
      if (string.IsNullOrEmpty(name))
        return -1;
      return UnsafeNativeMethods.CRhinoLinetypeTable_FindLinetype(m_doc.m_docId, name, ignoreDeletedLinetypes);
    }

    /// <summary>Find a linetype with a matching id</summary>
    /// <param name="id"></param>
    /// <param name="ignoreDeletedLinetypes">If true, deleted linetypes are not checked</param>
    /// <returns>
    /// >=0 index of the linetype with the given name
    /// -1  no linetype has the given name
    /// </returns>
    public int Find(Guid id, bool ignoreDeletedLinetypes)
    {
      return UnsafeNativeMethods.CRhinoLinetypeTable_FindLinetype2(m_doc.m_docId, id, ignoreDeletedLinetypes);
    }

    /// <summary>
    /// Adds a new linetype with specified definition to the linetype table.
    /// </summary>
    /// <param name="linetype">
    /// Definition of new linetype.  The information in linetype is copied.
    /// If linetype.Name is empty then a unique name of the form "Linetype 01"
    /// will be automatically created.
    /// </param>
    /// <returns>
    /// index of new linetype or -1 on error
    /// </returns>
    public int Add(DocObjects.Linetype linetype)
    {
      IntPtr pConstLinetype = linetype.ConstPointer();
      return UnsafeNativeMethods.CRhinoLinetypeTable_AddLinetype(m_doc.m_docId, pConstLinetype, false);
    }

    /// <summary>
    /// Adds a new linetype with specified definition to the linetype table.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="segmentLengths">Positive values are dashes, negative values are gaps</param>
    /// <returns>
    /// index of new linetype or -1 on error
    /// </returns>
    public int Add(string name, IEnumerable<double> segmentLengths)
    {
      using (Runtime.InteropWrappers.SimpleArrayDouble segs = new Rhino.Runtime.InteropWrappers.SimpleArrayDouble(segmentLengths))
      {
        IntPtr pSegs = segs.NonConstPointer();
        return UnsafeNativeMethods.CRhinoLinetypeTable_AddLinetype2(m_doc.m_docId, name, pSegs);
      }
    }

    /// <summary>
    /// Reference linetypes are not saved in files
    /// </summary>
    /// <param name="linetype"></param>
    /// <returns></returns>
    public int AddReferenceLinetype(DocObjects.Linetype linetype)
    {
      IntPtr pConstLinetype = linetype.ConstPointer();
      return UnsafeNativeMethods.CRhinoLinetypeTable_AddLinetype(m_doc.m_docId, pConstLinetype, true);
    }

    /// <summary>Modify linetype settings</summary>
    /// <param name="linetype">new settings.  This information is copied.</param>
    /// <param name="index">zero based index of linetype to set</param>
    /// <param name="quiet">
    /// if true, information message boxes pop up when illegal changes are attempted.
    /// </param>
    /// <returns>
    /// true if successful. false if linetype_index is out of range or the
    /// settings attempt to lock or hide the current linetype.
    /// </returns>
    public bool Modify(DocObjects.Linetype linetype, int index, bool quiet)
    {
      IntPtr pConstLinetype = linetype.ConstPointer();
      return UnsafeNativeMethods.CRhinoLinetypeTable_Modify(m_doc.m_docId, pConstLinetype, index, quiet);
    }

    /// <summary>
    /// If the linetype has been modified and the modifcation can be undone,
    /// then UndoModify() will restore the linetype to its previous state.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>
    /// true if this linetype had been modified and the modifications were undone
    /// </returns>
    public bool UndoModify(int index)
    {
      return UnsafeNativeMethods.CRhinoLinetypeTable_Un(m_doc.m_docId, index, true);
    }

    /// <summary>Deletes linetype</summary>
    /// <param name="index">zero based index of linetype to delete</param>
    /// <param name="quiet">
    /// If true, no warning message box appears if a linetype the
    /// linetype cannot be deleted because it is the current linetype
    /// or it contains active geometry.
    /// </param>
    /// <returns>
    /// true if successful. false if linetype_index is out of range or the
    /// linetype cannot be deleted because it is the current linetype or
    /// because it linetype is referenced by active geometry.
    /// </returns>
    public bool Delete(int index, bool quiet)
    {
      return UnsafeNativeMethods.CRhinoLinetypeTable_Delete(m_doc.m_docId, index, quiet);
    }

    /// <summary>Deletes multiple linetypes</summary>
    /// <param name="indices"></param>
    /// <param name="quiet"></param>
    /// <returns></returns>
    public bool Delete(IEnumerable<int> indices, bool quiet)
    {
      List<int> l = new List<int>(indices);
      int[] _indices = l.ToArray();
      return UnsafeNativeMethods.CRhinoLinetypeTable_Delete2(m_doc.m_docId, _indices.Length, _indices, quiet);
    }

    /// <summary>Undeletes a linetype that has been deleted</summary>
    /// <param name="index"></param>
    /// <returns>true if successful</returns>
    public bool Undelete(int index)
    {
      return UnsafeNativeMethods.CRhinoLinetypeTable_Un(m_doc.m_docId, index, false);
    }

    /// <summary>
    /// Gets unused linetype name used as default when creating new linetypes.
    /// </summary>
    /// <param name="ignoreDeleted">
    /// if this is true then may use a name used by a deleted linetype
    /// </param>
    /// <returns></returns>
    public string GetUnusedLinetypeName(bool ignoreDeleted)
    {
      using (Runtime.StringHolder sh = new Rhino.Runtime.StringHolder())
      {
        IntPtr pString = sh.NonConstPointer();
        UnsafeNativeMethods.CRhinoLinetypeTable_GetUnusedLinetypeName(m_doc.m_docId, ignoreDeleted, pString);
        return sh.ToString();
      }
    }

    /// <summary>
    /// Returns the text name of the continuous linetype
    /// </summary>
    public string ContinuousLinetypeName
    {
      get
      {
        IntPtr pName = UnsafeNativeMethods.CRhinoLinetypeTable_GetString(m_doc.m_docId, true);
        return Marshal.PtrToStringUni(pName);
      }
    }

    /// <summary>
    /// Returns the text name of the bylayer linetype
    /// </summary>
    public string ByLayerLinetypeName
    {
      get
      {
        IntPtr pName = UnsafeNativeMethods.CRhinoLinetypeTable_GetString(m_doc.m_docId, false);
        return Marshal.PtrToStringUni(pName);
      }
    }
  }
}
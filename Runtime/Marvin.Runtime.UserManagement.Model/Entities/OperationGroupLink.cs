﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using the Marvin template for generating a DbContext and Entities. 
// If you have any questions or suggestions for improvement regarding this code, contact Thomas Fuchs. I allways need feedback to improve.
// 
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Marvin.Model;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Marvin.Runtime.UserManagement.Model
{

    /// <summary>
    /// There are no comments for Marvin.Runtime.UserManagement.Model.OperationGroupLink in the schema.
    /// </summary>
    [System.Runtime.Serialization.DataContractAttribute(IsReference=true)]
    [System.Runtime.Serialization.KnownType(typeof(OperationGroup))]
    [System.Runtime.Serialization.KnownType(typeof(Operation))]
    public partial class OperationGroupLink : IEquatable<OperationGroupLink>, IMergeParent, IEntity    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public OperationGroupLink()
        {
        }


        #region Properties
    
        /// <summary>
        /// There are no comments for Id in the schema.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual long Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        private long _id;

    
        /// <summary>
        /// There are no comments for AccessType in the schema.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual OperationAccess AccessType
        {
            get
            {
                return _accessType;
            }
            set
            {
                if (_accessType != value)
                {
                    _accessType = value;
                    OnPropertyChanged("AccessType");
                }
            }
        }
        private OperationAccess _accessType;

    
        /// <summary>
        /// There are no comments for OperationGroupId in the schema.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual global::System.Nullable<long> OperationGroupId
        {
            get
            {
                return _operationGroupId;
            }
            set
            {
                if (_operationGroupId != value)
                {
                    _operationGroupId = value;
                    OnPropertyChanged("OperationGroupId");
                }
            }
        }
        private global::System.Nullable<long> _operationGroupId;

    
        /// <summary>
        /// There are no comments for OperationId in the schema.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual global::System.Nullable<long> OperationId
        {
            get
            {
                return _operationId;
            }
            set
            {
                if (_operationId != value)
                {
                    _operationId = value;
                    OnPropertyChanged("OperationId");
                }
            }
        }
        private global::System.Nullable<long> _operationId;


        #endregion

        #region Navigation Properties
    
        /// <summary>
        /// There are no comments for OperationGroup in the schema.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual OperationGroup OperationGroup
        {
            get;
            set;
        }
    
        /// <summary>
        /// There are no comments for Operation in the schema.
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual Operation Operation
        {
            get;
            set;
        }

        #endregion
        #region IEquatable
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            return Equals(other as OperationGroupLink); 
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The OperationGroupLink to compare with the current OperationGroupLink.</param>
        /// <returns><c>true</c> if the specified OperationGroupLink is equal to the current OperationGroupLink; otherwise, <c>false</c>.</returns>
        public bool Equals(OperationGroupLink other)
        {
            if((object)other == null)
                return false;
            
            // First look for Id, then compare references
            return (Id != 0 && Id == other.Id) || object.ReferenceEquals(this, other);
        }
     
        /// <summary>
        /// Compares two OperationGroupLink objects.
        /// </summary>
        /// <param name="a">The first OperationGroupLink to compare</param>
        /// <param name="b">The second OperationGroupLink to compare</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(OperationGroupLink  a, OperationGroupLink  b)
        {
            if((object)a == null && (object)b == null)
                return true;
            return (object)a != null && a.Equals(b);
        }

        /// <summary>
        /// Compares two OperationGroupLink objects.
        /// </summary>
        /// <param name="a">The first OperationGroupLink to compare</param>
        /// <param name="b">The second OperationGroupLink to compare</param>
        /// <returns><c>true</c> if the specified objects are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(OperationGroupLink  a, OperationGroupLink b)
        {
            return !(a == b);
        }

        #endregion
        
        /// <summary>
        /// Reference to merged child
        /// </summary>
        object IMergeParent.Child { get; set; }
    
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property value changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises a PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged(string propertyName) {

          if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

}
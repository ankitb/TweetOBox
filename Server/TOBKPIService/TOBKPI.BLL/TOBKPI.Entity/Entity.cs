﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TOBKPI.BLL
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Runtime.Serialization;
	using System.ComponentModel;
	using System;
	
	
	
	
	[Table(Name="dbo.TOBClientRegistration")]
	[DataContract()]
	public partial class TOBClientRegistration : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _RegistrationId;
		
		private System.DateTime _RegistrationDate;
		
		private EntitySet<TOBClientTimeTracker> _TOBClientTimeTrackers;
		
		private bool serializing;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnRegistrationIdChanging(System.Guid value);
    partial void OnRegistrationIdChanged();
    partial void OnRegistrationDateChanging(System.DateTime value);
    partial void OnRegistrationDateChanged();
    #endregion
		
		public TOBClientRegistration()
		{
			this.Initialize();
		}
		
		[Column(Storage="_RegistrationId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		[DataMember(Order=1)]
		public System.Guid RegistrationId
		{
			get
			{
				return this._RegistrationId;
			}
			set
			{
				if ((this._RegistrationId != value))
				{
					this.OnRegistrationIdChanging(value);
					this.SendPropertyChanging();
					this._RegistrationId = value;
					this.SendPropertyChanged("RegistrationId");
					this.OnRegistrationIdChanged();
				}
			}
		}
		
		[Column(Storage="_RegistrationDate", DbType="DateTime NOT NULL")]
		[DataMember(Order=2)]
		public System.DateTime RegistrationDate
		{
			get
			{
				return this._RegistrationDate;
			}
			set
			{
				if ((this._RegistrationDate != value))
				{
					this.OnRegistrationDateChanging(value);
					this.SendPropertyChanging();
					this._RegistrationDate = value;
					this.SendPropertyChanged("RegistrationDate");
					this.OnRegistrationDateChanged();
				}
			}
		}
		
		[Association(Name="FK_TOBClientTimeTracker_TOBClientRegistration", Storage="_TOBClientTimeTrackers", OtherKey="RegisterationId", DeleteRule="NO ACTION")]
		[DataMember(Order=3, EmitDefaultValue=false)]
		public EntitySet<TOBClientTimeTracker> TOBClientTimeTrackers
		{
			get
			{
				if ((this.serializing 
							&& (this._TOBClientTimeTrackers.HasLoadedOrAssignedValues == false)))
				{
					return null;
				}
				return this._TOBClientTimeTrackers;
			}
			set
			{
				this._TOBClientTimeTrackers.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_TOBClientTimeTrackers(TOBClientTimeTracker entity)
		{
			this.SendPropertyChanging();
			entity.TOBClientRegistration = this;
		}
		
		private void detach_TOBClientTimeTrackers(TOBClientTimeTracker entity)
		{
			this.SendPropertyChanging();
			entity.TOBClientRegistration = null;
		}
		
		private void Initialize()
		{
			this._TOBClientTimeTrackers = new EntitySet<TOBClientTimeTracker>(new Action<TOBClientTimeTracker>(this.attach_TOBClientTimeTrackers), new Action<TOBClientTimeTracker>(this.detach_TOBClientTimeTrackers));
			OnCreated();
		}
		
		[OnDeserializing()]
		[System.ComponentModel.EditorBrowsableAttribute(EditorBrowsableState.Never)]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
		
		[OnSerializing()]
		[System.ComponentModel.EditorBrowsableAttribute(EditorBrowsableState.Never)]
		public void OnSerializing(StreamingContext context)
		{
			this.serializing = true;
		}
		
		[OnSerialized()]
		[System.ComponentModel.EditorBrowsableAttribute(EditorBrowsableState.Never)]
		public void OnSerialized(StreamingContext context)
		{
			this.serializing = false;
		}
	}
	
	[Table(Name="dbo.TOBClientTimeTracker")]
	[DataContract()]
	public partial class TOBClientTimeTracker : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Guid _RegisterationId;
		
		private System.Nullable<System.DateTime> _TOBStartTime;
		
		private System.Nullable<System.DateTime> _TOBEndTime;
		
		private System.Nullable<long> _Duration;
		
		private EntityRef<TOBClientRegistration> _TOBClientRegistration;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnRegisterationIdChanging(System.Guid value);
    partial void OnRegisterationIdChanged();
    partial void OnTOBStartTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnTOBStartTimeChanged();
    partial void OnTOBEndTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnTOBEndTimeChanged();
    partial void OnDurationChanging(System.Nullable<long> value);
    partial void OnDurationChanged();
    #endregion
		
		public TOBClientTimeTracker()
		{
			this.Initialize();
		}
		
		[Column(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		[DataMember(Order=1)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[Column(Storage="_RegisterationId", DbType="UniqueIdentifier NOT NULL")]
		[DataMember(Order=2)]
		public System.Guid RegisterationId
		{
			get
			{
				return this._RegisterationId;
			}
			set
			{
				if ((this._RegisterationId != value))
				{
					if (this._TOBClientRegistration.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnRegisterationIdChanging(value);
					this.SendPropertyChanging();
					this._RegisterationId = value;
					this.SendPropertyChanged("RegisterationId");
					this.OnRegisterationIdChanged();
				}
			}
		}
		
		[Column(Storage="_TOBStartTime", DbType="DateTime")]
		[DataMember(Order=3)]
		public System.Nullable<System.DateTime> TOBStartTime
		{
			get
			{
				return this._TOBStartTime;
			}
			set
			{
				if ((this._TOBStartTime != value))
				{
					this.OnTOBStartTimeChanging(value);
					this.SendPropertyChanging();
					this._TOBStartTime = value;
					this.SendPropertyChanged("TOBStartTime");
					this.OnTOBStartTimeChanged();
				}
			}
		}
		
		[Column(Storage="_TOBEndTime", DbType="DateTime")]
		[DataMember(Order=4)]
		public System.Nullable<System.DateTime> TOBEndTime
		{
			get
			{
				return this._TOBEndTime;
			}
			set
			{
				if ((this._TOBEndTime != value))
				{
					this.OnTOBEndTimeChanging(value);
					this.SendPropertyChanging();
					this._TOBEndTime = value;
					this.SendPropertyChanged("TOBEndTime");
					this.OnTOBEndTimeChanged();
				}
			}
		}
		
		[Column(Storage="_Duration", DbType="BigInt")]
		[DataMember(Order=5)]
		public System.Nullable<long> Duration
		{
			get
			{
				return this._Duration;
			}
			set
			{
				if ((this._Duration != value))
				{
					this.OnDurationChanging(value);
					this.SendPropertyChanging();
					this._Duration = value;
					this.SendPropertyChanged("Duration");
					this.OnDurationChanged();
				}
			}
		}
		
		[Association(Name="FK_TOBClientTimeTracker_TOBClientRegistration", Storage="_TOBClientRegistration", ThisKey="RegisterationId", IsForeignKey=true)]
		public TOBClientRegistration TOBClientRegistration
		{
			get
			{
				return this._TOBClientRegistration.Entity;
			}
			set
			{
				TOBClientRegistration previousValue = this._TOBClientRegistration.Entity;
				if (((previousValue != value) 
							|| (this._TOBClientRegistration.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._TOBClientRegistration.Entity = null;
						previousValue.TOBClientTimeTrackers.Remove(this);
					}
					this._TOBClientRegistration.Entity = value;
					if ((value != null))
					{
						value.TOBClientTimeTrackers.Add(this);
						this._RegisterationId = value.RegistrationId;
					}
					else
					{
						this._RegisterationId = default(System.Guid);
					}
					this.SendPropertyChanged("TOBClientRegistration");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void Initialize()
		{
			this._TOBClientRegistration = default(EntityRef<TOBClientRegistration>);
			OnCreated();
		}
		
		[OnDeserializing()]
		[System.ComponentModel.EditorBrowsableAttribute(EditorBrowsableState.Never)]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
	}
}
#pragma warning restore 1591
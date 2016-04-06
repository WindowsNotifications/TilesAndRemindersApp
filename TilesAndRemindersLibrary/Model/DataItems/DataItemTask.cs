using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;

namespace TilesAndRemindersLibrary.Model.DataItems
{
    public class DataItemTask : BindableBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        

        private DateTimeOffset _startTime;
        public DateTimeOffset StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set { SetProperty(ref _isComplete, value); }
        }

        public void CopyFrom(DataItemTask task)
        {
            this.Id = task.Id;
            this.StartTime = task.StartTime;
            this.Title = task.Title;
            this.IsComplete = task.IsComplete;
        }

        public DataItemTask CreateDeepCopy()
        {
            var copied = new DataItemTask();
            copied.CopyFrom(this);
            return copied;
        }
    }
}

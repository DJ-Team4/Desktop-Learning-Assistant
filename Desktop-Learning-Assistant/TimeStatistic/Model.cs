using System;

namespace StatisticService.Model
{
    public class TaskPiece
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CloseTime { get; set; }
        public TimeSpan SpanTime
        {
            get
            {
                return CloseTime - StartTime;
            }
        }
        public string Path { get; set; }
        public string TaskType { get; set; }

        public override bool Equals(object obj)
        {
            TaskPiece tp2 = obj as TaskPiece;
            if (tp2.Name == this.Name && tp2.Detail == this.Detail && tp2.StartTime == this.StartTime)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Task
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public TimeSpan SpanTime { get; set; }
        public string Path { get; set; }
        public string TaskType { get; set; }

        public Task(TaskPiece tp)
        {
            this.Name = tp.Name;
            this.Detail = tp.Detail;
            this.SpanTime = tp.SpanTime;
            this.Path = tp.Path;
            this.TaskType = tp.TaskType;
        }

        public bool IsSameProc(TaskPiece tp)
        {
            if (tp.Name == this.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Add(TaskPiece tp)
        {
            this.SpanTime += tp.SpanTime;
        }

        public override string ToString()
        {
            return $"Name: {Name}\n\tDetail: {Detail}\n\t{SpanTime.TotalSeconds}s\n\tPath: {Path}\n\tTaskType: {TaskType}";
        }
    }
}

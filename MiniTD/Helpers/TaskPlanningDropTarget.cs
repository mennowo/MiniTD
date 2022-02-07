using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using MiniTD.DataTypes;
using MiniTD.ViewModels;

namespace MiniTD.Helpers
{
	public class TaskPlanningDropTarget : IDropTarget
	{
		public void DragEnter(IDropInfo dropInfo)
		{
			
		}

		public void DragOver(IDropInfo dropInfo)
		{
			// Call default DragOver method, cause most stuff should work by default
			GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
			if (dropInfo.TargetGroup == null)
			{
				dropInfo.Effects = DragDropEffects.None;
			}
		}

		public void DragLeave(IDropInfo dropInfo)
		{
			
		}

		public void Drop(IDropInfo dropInfo)
		{
			var group = dropInfo.TargetGroup;
			if (group != null && dropInfo.Data is MiniTaskViewModel item)
			{
				if (group.Items.Any())
				{
					var olddate = item.DateDue;
					var date = ((MiniTaskViewModel) group.Items.First()).DateDue;
					item.DateDue = new DateTime(date.Year, date.Month, date.Day, 
						olddate.Hour, olddate.Minute, olddate.Second);
					if (item.Status == MiniTaskStatus.Inactive ||
					    item.Status == MiniTaskStatus.ASAP)
					{
						item.Status = MiniTaskStatus.Scheduled;
					}
				}
			}
		}
	}
	
	public class CalendarPlanningDropTarget : IDropTarget
	{
		public void DragEnter(IDropInfo dropInfo)
		{
			
		}

		public void DragOver(IDropInfo dropInfo)
		{
			dropInfo.Effects = DragDropEffects.Move;
			dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
		}

		public void DragLeave(IDropInfo dropInfo)
		{
			
		}

		public void Drop(IDropInfo dropInfo)
		{
			var item = dropInfo.VisualTarget as UserControl;
			if (item?.DataContext is DisplayDay day && dropInfo.Data is MiniTaskViewModel task)
			{
				if (dropInfo.DragInfo.SourceCollection is ObservableCollection<MiniTaskViewModel> oldCollection)
				{
					oldCollection.Remove(task);
				}

				day.Tasks.Add(task);
				var olddate = task.DateDue;
				task.DateDue = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day,
					olddate.Hour, olddate.Minute, olddate.Second);
				if (task.Status == MiniTaskStatus.Inactive ||
				    task.Status == MiniTaskStatus.ASAP)
				{
					task.Status = MiniTaskStatus.Scheduled;
				}
			}
		}
	}
}
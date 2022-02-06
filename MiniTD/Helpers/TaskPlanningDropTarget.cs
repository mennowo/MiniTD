using System;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using MiniTD.DataTypes;
using MiniTD.ViewModels;

namespace MiniTD.Helpers
{
	public class TaskPlanningDropTarget : IDropTarget
	{
		public void DragOver(IDropInfo dropInfo)
		{
			// Call default DragOver method, cause most stuff should work by default
			GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
			if (dropInfo.TargetGroup == null)
			{
				dropInfo.Effects = DragDropEffects.None;
			}
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
}
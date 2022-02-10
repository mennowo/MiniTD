using System;
using System.Collections;
using MiniTD.ViewModels;

namespace MiniTD.Helpers
{
	public class MiniTaskViewModelDueDateComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			if(x is MiniTaskViewModel t1 && y is MiniTaskViewModel t2)
			{
				var c = string.Compare(t1.Title, t2.Title, StringComparison.Ordinal);
				if (c == 0)
				{
					c = t1.DateDue.CompareTo(t2.DateDue);
				}
				return c;
			}
			throw new InvalidCastException();
		}
	}
}
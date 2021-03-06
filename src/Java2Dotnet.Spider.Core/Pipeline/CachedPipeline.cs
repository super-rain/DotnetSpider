﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Java2Dotnet.Spider.Log;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class CachedPipeline : BasePipeline
	{
		private readonly List<ResultItems> _cached = new List<ResultItems>();
		public int CachedSize { get; set; } = 1;

		protected abstract void Process(List<ResultItems> resultItemsList);

		public override void Process(ResultItems resultItems)
		{
			lock (this)
			{
				_cached.Add(resultItems);

				if (_cached.Count >= CachedSize)
				{
					List<ResultItems> result = new List<ResultItems>();

					result.AddRange(_cached);
					_cached.Clear();

					// 做成异步
					Process(result);
				}
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_cached.Count > 0)
			{
				Process(_cached);
				_cached.Clear();
			}
		}
	}
}

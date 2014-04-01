using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublishITService.Resources {
	public interface IMediaParser
	{
		void StoreMedia(String filename);
	}
}
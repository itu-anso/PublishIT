using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PublishITService.Resources {
	public interface IMediaParser
	{
		void StoreMedia(byte[] mediaStream, RemoteFileInfo mediaInfo, IPublishITEntities entities);
	}
}
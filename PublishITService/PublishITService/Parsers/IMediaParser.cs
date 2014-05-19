using System.IO;

namespace PublishITService.Parsers {
	public interface IMediaParser
	{
		void StoreMedia(Stream mediaStream, RemoteFileInfo mediaInfo, IPublishITEntities entities);
	}
}
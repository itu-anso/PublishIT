namespace PublishITService.Parsers {
	public interface IMediaParser
	{
		void StoreMedia(byte[] mediaStream, RemoteFileInfo mediaInfo, IPublishITEntities entities);
	}
}
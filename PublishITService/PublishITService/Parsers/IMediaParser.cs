using System.IO;

namespace PublishITService.Parsers {
	public interface IMediaParser
	{
        /// <summary>
        /// Interface for storing a media into a database. 
        /// </summary>
        /// <param name="mediaInfo">Object containing different info relevant for a media object.</param>
        /// <param name="entities">Interface for every entity class in the database.</param>
		void StoreMedia(RemoteFileInfo mediaInfo, IPublishITEntities entities);
	}
}
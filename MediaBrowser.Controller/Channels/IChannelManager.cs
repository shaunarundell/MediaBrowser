﻿using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Controller.Channels
{
    public interface IChannelManager
    {
        /// <summary>
        /// Adds the parts.
        /// </summary>
        /// <param name="channels">The channels.</param>
        void AddParts(IEnumerable<IChannel> channels);

        /// <summary>
        /// Gets the channels.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{QueryResult{BaseItemDto}}.</returns>
        Task<QueryResult<BaseItemDto>> GetChannels(ChannelQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the channel items.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{QueryResult{BaseItemDto}}.</returns>
        Task<QueryResult<BaseItemDto>> GetChannelItems(ChannelItemQuery query, CancellationToken cancellationToken);
    }
}

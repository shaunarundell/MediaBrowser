﻿using System;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Sorting;
using MediaBrowser.Model.Querying;

namespace MediaBrowser.Server.Implementations.Sorting
{
    class AiredEpisodeOrderComparer : IBaseItemComparer
    {
        /// <summary>
        /// Compares the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Int32.</returns>
        public int Compare(BaseItem x, BaseItem y)
        {
            var val = DateTime.Compare(x.PremiereDate ?? DateTime.MinValue, y.PremiereDate ?? DateTime.MinValue);

            if (val != 0)
            {
                return val;
            }

            var episode1 = x as Episode;
            var episode2 = y as Episode;

            if (episode1 == null)
            {
                if (episode2 == null)
                {
                    return 0;
                }

                return 1;
            }

            if (episode2 == null)
            {
                return -1;
            }

            return Compare(episode1, episode2);
        }

        private int Compare(Episode x, Episode y)
        {
            var isXSpecial = (x.ParentIndexNumber ?? -1) == 0;
            var isYSpecial = (y.ParentIndexNumber ?? -1) == 0;

            if (isXSpecial && isYSpecial)
            {
                return CompareSpecials(x, y);
            }

            if (!isXSpecial && !isYSpecial)
            {
                return CompareEpisodes(x, y);
            }

            if (!isXSpecial && isYSpecial)
            {
                return CompareEpisodeToSpecial(x, y);
            }

            return CompareEpisodeToSpecial(x, y) * -1;
        }

        private int CompareEpisodeToSpecial(Episode x, Episode y)
        {
            var xSeason = x.ParentIndexNumber ?? -1;
            var ySeason = y.AirsAfterSeasonNumber ?? y.AirsBeforeSeasonNumber ?? -1;

            if (xSeason != ySeason)
            {
                return xSeason.CompareTo(ySeason);
            }

            // Now we know they have the same season

            // Compare episode number

            // Add 1 to to non-specials to account for AirsBeforeEpisodeNumber
            var xEpisode = (x.IndexNumber ?? 0) * 1000 + 1;
            var yEpisode = (y.AirsBeforeEpisodeNumber ?? 0) * 1000;

            return xEpisode.CompareTo(yEpisode);
        }

        private int CompareSpecials(Episode x, Episode y)
        {
            return GetSpecialCompareValue(x).CompareTo(GetSpecialCompareValue(y));
        }

        private int GetSpecialCompareValue(Episode item)
        {
            // First sort by season number
            // Since there are three sort orders, pad with 9 digits (3 for each, figure 1000 episode buffer should be enough)
            var val = (item.AirsBeforeSeasonNumber ?? item.AirsAfterSeasonNumber ?? 0) * 1000000000;

            // Second sort order is if it airs after the season
            if (item.AirsAfterSeasonNumber.HasValue)
            {
                val += 1000000;
            }

            // Third level is the episode number
            val += (item.AirsBeforeEpisodeNumber ?? 0) * 1000;

            // Finally, if that's still the same, last resort is the special number itself
            val += item.IndexNumber ?? 0;

            return val;
        }

        private int CompareEpisodes(Episode x, Episode y)
        {
            var xValue = ((x.ParentIndexNumber ?? -1) * 1000) + (x.IndexNumber ?? -1);
            var yValue = ((y.ParentIndexNumber ?? -1) * 1000) + (y.IndexNumber ?? -1);

            return xValue.CompareTo(yValue);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return ItemSortBy.AiredEpisodeOrder; }
        }
    }
}
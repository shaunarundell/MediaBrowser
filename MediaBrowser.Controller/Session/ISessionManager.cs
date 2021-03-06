﻿using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Controller.Session
{
    /// <summary>
    /// Interface ISessionManager
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Occurs when [playback start].
        /// </summary>
        event EventHandler<PlaybackProgressEventArgs> PlaybackStart;

        /// <summary>
        /// Occurs when [playback progress].
        /// </summary>
        event EventHandler<PlaybackProgressEventArgs> PlaybackProgress;

        /// <summary>
        /// Occurs when [playback stopped].
        /// </summary>
        event EventHandler<PlaybackStopEventArgs> PlaybackStopped;

        /// <summary>
        /// Occurs when [session started].
        /// </summary>
        event EventHandler<SessionEventArgs> SessionStarted;

        /// <summary>
        /// Occurs when [session ended].
        /// </summary>
        event EventHandler<SessionEventArgs> SessionEnded;
        
        /// <summary>
        /// Gets the sessions.
        /// </summary>
        /// <value>The sessions.</value>
        IEnumerable<SessionInfo> Sessions { get; }

        /// <summary>
        /// Adds the parts.
        /// </summary>
        /// <param name="sessionFactories">The session factories.</param>
        void AddParts(IEnumerable<ISessionControllerFactory> sessionFactories);

        /// <summary>
        /// Logs the user activity.
        /// </summary>
        /// <param name="clientType">Type of the client.</param>
        /// <param name="appVersion">The app version.</param>
        /// <param name="deviceId">The device id.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        Task<SessionInfo> LogSessionActivity(string clientType, string appVersion, string deviceId, string deviceName, string remoteEndPoint, User user);

        /// <summary>
        /// Used to report that playback has started for an item
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns>Task.</returns>
        Task OnPlaybackStart(PlaybackInfo info);

        /// <summary>
        /// Used to report playback progress for an item
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Task OnPlaybackProgress(PlaybackProgressInfo info);

        /// <summary>
        /// Used to report that playback has ended for an item
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Task OnPlaybackStopped(PlaybackStopInfo info);

        /// <summary>
        /// Reports the session ended.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>Task.</returns>
        Task ReportSessionEnded(Guid sessionId);

        /// <summary>
        /// Gets the session info dto.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>SessionInfoDto.</returns>
        SessionInfoDto GetSessionInfoDto(SessionInfo session);

        /// <summary>
        /// Sends the general command.
        /// </summary>
        /// <param name="controllingSessionId">The controlling session identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendGeneralCommand(Guid controllingSessionId, Guid sessionId, GeneralCommand command, CancellationToken cancellationToken);
        
        /// <summary>
        /// Sends the message command.
        /// </summary>
        /// <param name="controllingSessionId">The controlling session identifier.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendMessageCommand(Guid controllingSessionId, Guid sessionId, MessageCommand command, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the play command.
        /// </summary>
        /// <param name="controllingSessionId">The controlling session identifier.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendPlayCommand(Guid controllingSessionId, Guid sessionId, PlayRequest command, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the browse command.
        /// </summary>
        /// <param name="controllingSessionId">The controlling session identifier.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendBrowseCommand(Guid controllingSessionId, Guid sessionId, BrowseRequest command, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the playstate command.
        /// </summary>
        /// <param name="controllingSessionId">The controlling session identifier.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendPlaystateCommand(Guid controllingSessionId, Guid sessionId, PlaystateRequest command, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the restart required message.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendRestartRequiredNotification(CancellationToken cancellationToken);

        /// <summary>
        /// Sends the server shutdown notification.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendServerShutdownNotification(CancellationToken cancellationToken);

        /// <summary>
        /// Sends the server restart notification.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendServerRestartNotification(CancellationToken cancellationToken);

        /// <summary>
        /// Adds the additional user.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void AddAdditionalUser(Guid sessionId, Guid userId);

        /// <summary>
        /// Removes the additional user.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void RemoveAdditionalUser(Guid sessionId, Guid userId);

        /// <summary>
        /// Authenticates the new session.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <param name="clientType">Type of the client.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <returns>Task{SessionInfo}.</returns>
        Task<SessionInfo> AuthenticateNewSession(User user, string password, string clientType, string appVersion, string deviceId, string deviceName, string remoteEndPoint);

        /// <summary>
        /// Reports the capabilities.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="capabilities">The capabilities.</param>
        void ReportCapabilities(Guid sessionId, SessionCapabilities capabilities);
    }
}
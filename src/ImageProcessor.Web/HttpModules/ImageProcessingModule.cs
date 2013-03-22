﻿// -----------------------------------------------------------------------
// <copyright file="ImageProcessingModule.cs" company="James South">
//     Copyright (c) James South.
//     Dual licensed under the MIT or GPL Version 2 licenses.
// </copyright>
// -----------------------------------------------------------------------

namespace ImageProcessor.Web.HttpModules
{
    #region Using
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Hosting;
    using ImageProcessor.Helpers.Extensions;
    using ImageProcessor.Imaging;
    using ImageProcessor.Web.Caching;
    using ImageProcessor.Web.Config;
    using ImageProcessor.Web.Helpers;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Processes any image requests within the web application.
    /// </summary>
    public class ImageProcessingModule : IHttpModule
    {
        #region Fields
        /// <summary>
        /// The key for storing the response type of the current image.
        /// </summary>
        private const string CachedResponseTypeKey = "CACHED_IMAGE_RESPONSE_TYPE";

        /// <summary>
        /// The value to prefix any remote image requests with to ensure they get captured.
        /// </summary>
        private static readonly string RemotePrefix = ImageProcessorConfig.Instance.RemotePrefix;

        /// <summary>
        /// The object to lock against.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The assembly version.
        /// </summary>
        private static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// A value indicating whether the application has started.
        /// </summary>
        private static bool hasModuleInitialized;
        #endregion

        #region IHttpModule Members
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpApplication"/> that provides 
        /// access to the methods, properties, and events common to all 
        /// application objects within an ASP.NET application
        /// </param>
        public void Init(HttpApplication context)
        {
            if (!hasModuleInitialized)
            {
                lock (SyncRoot)
                {
                    if (!hasModuleInitialized)
                    {
                        Cache.CreateDirectoriesAsync();
                        //DiskCache.CreateCacheDirectories();
                        hasModuleInitialized = true;
                    }
                }
            }

            context.BeginRequest += this.ContextBeginRequest;
            context.PreSendRequestHeaders += this.ContextPreSendRequestHeaders;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose.
        }
        #endregion

        /// <summary>
        /// Occurs as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        private void ContextBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            this.ProcessImage(context);
        }

        /// <summary>
        /// Occurs just before ASP.NET send HttpHeaders to the client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        private void ContextPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;

            object responseTypeObject = context.Items[CachedResponseTypeKey];

            if (responseTypeObject != null)
            {
                string responseType = (string)responseTypeObject;

                // Set the headers
                this.SetHeaders(context, responseType);

                context.Items[CachedResponseTypeKey] = null;
            }
        }

        #region Private
        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="context">
        /// the <see cref="T:System.Web.HttpContext">HttpContext</see> object that provides 
        /// references to the intrinsic server objects 
        /// </param>
        private void ProcessImage(HttpContext context)
        {
            this.ProcessImageAsync(context);
        }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="context">
        /// the <see cref="T:System.Web.HttpContext">HttpContext</see> object that provides 
        /// references to the intrinsic server objects 
        /// </param>
        private /*async*/ Task ProcessImageAsync(HttpContext context)
        {
            return this.ProcessImageAsyncTask(context).ToTask();
        }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="context">
        /// the <see cref="T:System.Web.HttpContext">HttpContext</see> object that provides 
        /// references to the intrinsic server objects 
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Task}"/>.
        /// </returns>
        private IEnumerable<Task> ProcessImageAsyncTask(HttpContext context)
        {
            // Is this a remote file.
            bool isRemote = context.Request.Path.Equals(RemotePrefix, StringComparison.OrdinalIgnoreCase);
            string requestPath = string.Empty;
            string queryString = string.Empty;

            if (isRemote)
            {
                // We need to split the querystring to get the actual values we want.
                string urlDecode = HttpUtility.UrlDecode(context.Request.QueryString.ToString());

                if (urlDecode != null)
                {
                    string[] paths = urlDecode.Split('?');

                    requestPath = paths[0];

                    if (paths.Length > 1)
                    {
                        queryString = paths[1];
                    }
                }
            }
            else
            {
                requestPath = HostingEnvironment.MapPath(context.Request.Path);
                queryString = HttpUtility.UrlDecode(context.Request.QueryString.ToString());
            }

            // Only process requests that pass our sanitizing filter.
            if (ImageUtils.IsValidImageExtension(requestPath) && !string.IsNullOrWhiteSpace(queryString))
            {
                if (this.FileExists(requestPath, isRemote))
                {

                    string fullPath = string.Format("{0}?{1}", requestPath, queryString);
                    string imageName = Path.GetFileName(requestPath);

                    // Create a new cache to help process and cache the request.
                    Cache cache = new Cache(requestPath, fullPath, imageName, isRemote);

                    // Is the file new or updated?
                    Task<bool> isUpdatedTask = cache.isNewOrUpdatedFileAsync();
                    yield return isUpdatedTask;
                    bool isNewOrUpdated = isUpdatedTask.Result;

                    // Only process if the file has been updated.
                    if (isNewOrUpdated)
                    {
                        // Process the image.
                        using (ImageFactory imageFactory = new ImageFactory())
                        {
                            if (isRemote)
                            {
                                Uri uri = new Uri(requestPath);

                                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

                                Task<WebResponse> responseTask = webRequest.GetResponseAsync();
                                yield return responseTask;
                                //RemoteFile remoteFile = new RemoteFile(uri, false);

                                //Task<WebResponse> getWebResponseTask = remoteFile.GetWebResponseAsync();
                                //yield return getWebResponseTask;

                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    using (WebResponse response = responseTask.Result)
                                    {
                                        using (Stream responseStream = response.GetResponseStream())
                                        {
                                            if (responseStream != null)
                                            {
                                                // Trim the cache.
                                                Task trimCachedFoldersTask = cache.TrimCachedFoldersAsync();
                                                yield return trimCachedFoldersTask;

                                                responseStream.CopyTo(memoryStream);

                                                imageFactory.Load(memoryStream)
                                                    .AddQueryString(queryString)
                                                    .Format(ImageUtils.GetImageFormat(imageName))
                                                    .AutoProcess().Save(cache.CachedPath);

                                                // Ensure that the LastWriteTime property of the source and cached file match.
                                                Task<DateTime> setCachedLastWriteTimeTask = cache.SetCachedLastWriteTimeAsync();
                                                yield return setCachedLastWriteTimeTask;
                                                DateTime dateTime = setCachedLastWriteTimeTask.Result;

                                                // Add to the cache.
                                                Task addImageToCacheTask = cache.AddImageToCacheAsync(dateTime);
                                                yield return addImageToCacheTask;
                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                // Trim the cache.
                                Task trimCachedFoldersTask = cache.TrimCachedFoldersAsync();
                                yield return trimCachedFoldersTask;

                                imageFactory.Load(fullPath).AutoProcess().Save(cache.CachedPath);

                                // Ensure that the LastWriteTime property of the source and cached file match.
                                Task<DateTime> setCachedLastWriteTimeTask = cache.SetCachedLastWriteTimeAsync();
                                yield return setCachedLastWriteTimeTask;
                                DateTime dateTime = setCachedLastWriteTimeTask.Result;

                                // Add to the cache.
                                Task addImageToCacheTask = cache.AddImageToCacheAsync(dateTime);
                                yield return addImageToCacheTask;

                            }
                        }
                    }

                    context.Items[CachedResponseTypeKey] = ImageUtils.GetResponseType(imageName).ToDescription();

                    // The cached file is valid so just rewrite the path.
                    context.RewritePath(cache.GetVirtualPath(cache.CachedPath, context.Request), false);
                    yield break;

                }
            }

            yield break;
        }


        /// <summary>
        /// returns a value indicating whether a file exists.
        /// </summary>
        /// <param name="path">The path to the file to check.</param>
        /// <param name="isRemote">Whether the file is remote.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        /// <remarks>If the file is remote the method will always return true.</remarks>
        private bool FileExists(string path, bool isRemote)
        {
            if (isRemote)
            {
                return true;
            }

            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Exists;
        }

        /// <summary>
        /// This will make the browser and server keep the output
        /// in its cache and thereby improve performance.
        /// See http://en.wikipedia.org/wiki/HTTP_ETag
        /// </summary>
        /// <param name="context">
        /// the <see cref="T:System.Web.HttpContext">HttpContext</see> object that provides 
        /// references to the intrinsic server objects 
        /// </param>
        /// <param name="responseType">The HTTP MIME type to to send.</param>
        private void SetHeaders(HttpContext context, string responseType)
        {
            HttpResponse response = context.Response;

            response.ContentType = responseType;

            response.AddHeader("Image-Served-By", "ImageProcessor/" + AssemblyVersion);

            HttpCachePolicy cache = response.Cache;

            cache.VaryByHeaders["Accept-Encoding"] = true;

            int maxDays = DiskCache.MaxFileCachedDuration;

            cache.SetExpires(DateTime.Now.ToUniversalTime().AddDays(maxDays));
            cache.SetMaxAge(new TimeSpan(maxDays, 0, 0, 0));
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            string incomingEtag = context.Request.Headers["If-None-Match"];

            cache.SetCacheability(HttpCacheability.Public);

            if (incomingEtag == null)
            {
                return;
            }

            response.Clear();
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.SuppressContent = true;
        }
        #endregion
    }
}
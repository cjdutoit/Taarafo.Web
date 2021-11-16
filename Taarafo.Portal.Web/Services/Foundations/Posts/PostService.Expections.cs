﻿using RESTFulSense.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Taarafo.Portal.Web.Models.Posts;
using Taarafo.Portal.Web.Models.Posts.Exceptions;
using Xeptions;

namespace Taarafo.Portal.Web.Services.Foundations.Posts
{
    public partial class PostService
    {
        private delegate ValueTask<Post> ReturningPostFunction();

        private async ValueTask<Post> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (InvalidPostException invalidPostException)
            {
                throw CreateAndLogValidationException(invalidPostException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedPostDependencyException = 
                    new FailedPostDependencyException(httpRequestException);

                throw CreateAndLogCritialDependencyException(failedPostDependencyException);
            }
            catch(HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
            {
                var failedPostDependencyException = 
                    new FailedPostDependencyException(httpResponseUrlNotFoundException);

                throw CreateAndLogCritialDependencyException(failedPostDependencyException);
            }
            catch(HttpResponseUnauthorizedException httpResponseUnauthorizedException)
            {
                var failedPostDependencyException =
                    new FailedPostDependencyException(httpResponseUnauthorizedException);

                throw CreateAndLogCritialDependencyException(failedPostDependencyException);
            }
            catch(HttpResponseNotFoundException httpResponseNotFoundException)
            {
                var notFoundPostException = 
                    new NotFoundPostException(httpResponseNotFoundException);

                throw CreateAndLogDependencyValidationException(notFoundPostException);
            }
            catch (HttpResponseBadRequestException httpResponseBadRequestException)
            {
                var invalidPostException =
                    new InvalidPostException(
                        httpResponseBadRequestException,
                        httpResponseBadRequestException.Data);

                throw CreateAndLogDependencyValidationException(invalidPostException);
            }
            catch(HttpResponseLockedException httpLockedException)
            {
                var lockedPostException =
                    new LockedPostException(httpLockedException);

                throw CreateAndLogDependencyValidationException(lockedPostException);
            }
            catch (HttpResponseException httpResponseException)
            {
                var failedPostDependencyException =
                    new FailedPostDependencyException(httpResponseException);

                throw CreateAndLogDependencyException(failedPostDependencyException);
            }
            catch (Exception exception)
            {
                var failedPostServiceException =
                    new FailedPostServiceException(exception);

                throw CreateAndLogPostServiceException(failedPostServiceException);
            }
        }

        private PostDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var postDependencyValidationException = 
                new PostDependencyValidationException(exception);

            this.loggingBroker.LogError(postDependencyValidationException);

            return postDependencyValidationException;
        }

        private PostDependencyException CreateAndLogCritialDependencyException(Xeption exception)
        {
            var postDependencyException =
                new PostDependencyException(exception);
            
            this.loggingBroker.LogCritical(postDependencyException);

            return postDependencyException;
        }

        private PostValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var postValidationException =
                new PostValidationException(exception);

            this.loggingBroker.LogError(postValidationException);

            return postValidationException;
        }

        private PostDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var postDependencyException =
                new PostDependencyException(exception);

            this.loggingBroker.LogError(postDependencyException);

            return postDependencyException;
        }

        private PostServiceException CreateAndLogPostServiceException(Xeption exception)
        {
            var postServiceException =
                new PostServiceException(exception);

            this.loggingBroker.LogError(postServiceException);

            return postServiceException;
        }
    }
}
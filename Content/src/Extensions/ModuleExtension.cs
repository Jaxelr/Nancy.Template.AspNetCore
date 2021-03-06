﻿using System;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace Nancy.Template.WebService.Extensions
{
    public static class ModuleExtensions
    {
        private static string ModelBindingErrorMessage => "The model is not binding to the request";

        public static void Get<TOut>(this NancyModule module, string path, Func<TOut> handler)
            => module.Get(path, path, handler);

        public static void Post<TOut>(this NancyModule module, string path, Func<TOut> handler)
            => module.Post(path, path, handler);

        public static void Delete<TOut>(this NancyModule module, string path, Func<TOut> handler)
            => module.Delete(path, path, handler);

        public static void Get<TOut>(this NancyModule module, string name, string path, Func<TOut> handler)
            => module.Get(path, _ => RunHandler(module, handler), name: name);

        public static void Post<TOut>(this NancyModule module, string name, string path, Func<TOut> handler)
            => module.Post(path, _ => RunHandler(module, handler), name: name);

        public static void Delete<TOut>(this NancyModule module, string name, string path, Func<TOut> handler)
            => module.Delete(path, _ => RunHandler(module, handler), name: name);

        public static void Get<TIn, TOut>(this NancyModule module, string path, Func<TIn, TOut> handler)
            => module.Get(path, path, handler);

        public static void Post<TIn, TOut>(this NancyModule module, string path, Func<TIn, TOut> handler)
            => module.Post(path, path, handler);

        public static void Delete<TIn, TOut>(this NancyModule module, string path, Func<TIn, TOut> handler)
            => module.Delete(path, path, handler);

        public static void Get<TIn, TOut>(this NancyModule module, string name, string path, Func<TIn, TOut> handler)
            => module.Get(path, _ => RunHandler(module, handler), name: name);

        public static void Post<TIn, TOut>(this NancyModule module, string name, string path, Func<TIn, TOut> handler)
            => module.Post(path, _ => RunHandler(module, handler), name: name);

        public static void Delete<TIn, TOut>(this NancyModule module, string name, string path, Func<TIn, TOut> handler)
            => module.Delete(path, _ => RunHandler(module, handler), name: name);

        public static void Get<TIn>(this NancyModule module, string name, string path, Func<TIn, Task<object>> handler)
            => module.Get(path, _ => RunHandlerAsync(module, handler), name: name);

        public static void Post<TIn>(this NancyModule module, string name, string path, Func<TIn, Task<object>> handler)
            => module.Post(path, _ => RunHandlerAsync(module, handler), name: name);

        public static void Delete<TIn>(this NancyModule module, string name, string path, Func<TIn, Task<object>> handler)
            => module.Delete(path, _ => RunHandlerAsync(module, handler), name: name);

        public static object RunHandler<TOut>(this NancyModule module, Func<TOut> handler)
        {
            try
            {
                return handler();
            }
            catch (Exception Ex)
            {
                return module.Negotiate.RespondWithFailure(Ex);
            }
        }

        public static async Task<object> RunHandlerAsync(this NancyModule module, Func<Task<object>> handler)
        {
            try
            {
                return await handler().ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                return module.Negotiate.RespondWithFailure(Ex);
            }
        }

        public static object RunHandler<TIn, TOut>(this NancyModule module, Func<TIn, TOut> handler)
        {
            try
            {
                TIn model;
                try
                {
                    model = module.BindAndValidate<TIn>();
                    if (!module.ModelValidationResult.IsValid)
                    {
                        return module.Negotiate.RespondWithValidationFailure(module.ModelValidationResult);
                    }
                }
                catch (ModelBindingException)
                {
                    return module.Negotiate.RespondWithValidationFailure(ModelBindingErrorMessage);
                }

                return handler(model);
            }
            catch (Exception Ex)
            {
                return module.Negotiate.RespondWithFailure(Ex);
            }
        }

        public static async Task<object> RunHandlerAsync<TIn>(this NancyModule module, Func<TIn, Task<object>> handler)
        {
            try
            {
                TIn model;
                try
                {
                    model = module.BindAndValidate<TIn>();
                    if (!module.ModelValidationResult.IsValid)
                    {
                        return module.Negotiate.RespondWithValidationFailure(module.ModelValidationResult);
                    }
                }
                catch (ModelBindingException)
                {
                    return module.Negotiate.RespondWithValidationFailure(ModelBindingErrorMessage);
                }

                return await handler(model).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                return module.Negotiate.RespondWithFailure(Ex);
            }
        }
    }
}

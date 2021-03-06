﻿using Pchp.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Pchp.Library
{
    public static class Errors
    {
        #region Constants

        /// <summary>Error.</summary>
        public const int E_ERROR = (int)PhpError.E_ERROR;
        /// <summary>Warning.</summary>
        public const int E_WARNING = (int)PhpError.E_WARNING;
        /// <summary>Parse error.</summary>
        public const int E_PARSE = (int)PhpError.E_PARSE;
        /// <summary>Notice.</summary>
        public const int E_NOTICE = (int)PhpError.E_NOTICE;
        /// <summary>Core error.</summary>
        public const int E_CORE_ERROR = (int)PhpError.E_CORE_ERROR;
        /// <summary>Core warning.</summary>
        public const int E_CORE_WARNING = (int)PhpError.E_CORE_WARNING;
        /// <summary>Compile error.</summary>
        public const int E_COMPILE_ERROR = (int)PhpError.E_COMPILE_ERROR;
        /// <summary>Compile warning.</summary>
        public const int E_COMPILE_WARNING = (int)PhpError.E_COMPILE_WARNING;
        /// <summary>User error.</summary>
        public const int E_USER_ERROR = (int)PhpError.E_USER_ERROR;
        /// <summary>User warning.</summary>
        public const int E_USER_WARNING = (int)PhpError.E_USER_WARNING;
        /// <summary>User notice.</summary>
        public const int E_USER_NOTICE = (int)PhpError.E_USER_NOTICE;
        /// <summary>Strict error.</summary>
        public const int E_STRICT = (int)PhpError.E_STRICT;
        /// <summary>E_RECOVERABLE_ERROR error.</summary>
        public const int E_RECOVERABLE_ERROR = (int)PhpError.E_RECOVERABLE_ERROR;
        /// <summary>Deprecated error.</summary>
        public const int E_DEPRECATED = (int)PhpError.E_DEPRECATED;
        /// <summary>Deprecated error.</summary>
        public const int E_USER_DEPRECATED = (int)PhpError.E_USER_DEPRECATED;

        /// <summary>All errors but strict.</summary>
        public const int E_ALL = (int)PhpErrorSets.AllButStrict;

        #endregion

        #region ErrorContext

        /// <summary>
        /// Current context errors configuration.
        /// </summary>
        class ErrorContext
        {
            /// <summary>
            /// Stores user error handlers which has been rewritten by a new one.
            /// </summary>
            public Stack<ErrorHandlerRecord> OldUserErrorHandlers = null;

            /// <summary>
            /// Stores user exception handlers which has been rewritten by a new one.
            /// </summary>
            public Stack<ErrorHandlerRecord> OldUserExceptionHandlers = null;

            /// <summary>
            /// Errors to be reported to user.
            /// Ignored when <see cref="Context.ErrorReportingDisabled"/> is <c>true</c>.
            /// </summary>
            public PhpError ReportErrors = (PhpError)PhpErrorSets.All;
        }

        #endregion

        #region error_reporting, set_error_handler, restore_error_handler, set_exception_handler, restore_exception_handler

        /// <summary>
        /// Retrieves the current error reporting level.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <returns>
        /// The bitmask of error types which are reported. Returns 0 if error reporting is disabled
        /// by means of @ operator.
        /// </returns>
        public static int error_reporting(Context ctx)
        {
            return ctx.ErrorReportingDisabled ? 0 : (int)ctx.GetStatic<ErrorContext>().ReportErrors;
        }

        /// <summary>
        /// Sets a new level of error reporting.
        /// </summary>
        /// <param name="ctx">Current runtime context.</param>
        /// <param name="level">The new level.</param>
        /// <returns>The original level.</returns>
        public static int error_reporting(Context ctx, int level)
        {
            if ((level & (int)PhpErrorSets.All) == 0 && level != 0)
            {
                //PhpException.InvalidArgument("level");
                throw new ArgumentException(nameof(level));
            }

            var errctx = ctx.GetStatic<ErrorContext>();
            var result = (int)errctx.ReportErrors;
            errctx.ReportErrors = (PhpError)level & (PhpError)PhpErrorSets.All;
            return result;
        }

        /// <summary>
        /// Internal record in the error handler stack.
        /// </summary>
        [DebuggerDisplay("{ErrorHandler,nq}, {ErrorTypes}")]
        class ErrorHandlerRecord
        {
            /// <summary>
            /// Error handler callback.
            /// </summary>
            public readonly PhpCallback ErrorHandler;

            /// <summary>
            /// Error types to be handled.
            /// </summary>
            public readonly PhpError ErrorTypes;

            /// <summary>
            /// Public constructor of the class.
            /// </summary>
            /// <param name="handler">Error handler callback.</param>
            /// <param name="errors">Error types to be handled.</param>
            public ErrorHandlerRecord(PhpCallback handler, PhpError errors)
            {
                ErrorHandler = handler;
                ErrorTypes = errors;
            }
        }

        ///// <summary>
        ///// Sets user defined handler to handle errors.
        ///// </summary>
        ///// <param name="caller">The class context used to bind the callback.</param>
        ///// <param name="newHandler">The user callback called to handle an error.</param>
        ///// <returns>
        ///// The PHP representation of previous user handler, <B>null</B> if there is no user one, or 
        ///// <B>false</B> if <paramref name="newHandler"/> is invalid or empty.
        ///// </returns>
        ///// <remarks>
        ///// Stores old user handlers on the stack so that it is possible to 
        ///// go back to arbitrary previous user handler.
        ///// </remarks>
        //public static object set_error_handler(PHP.Core.Reflection.DTypeDesc caller, IPhpCallable newHandler)
        //{
        //    return set_error_handler(caller, newHandler, (int)PhpErrorSet.Handleable);
        //}

        ///// <summary>
        ///// Sets user defined handler to handle errors.
        ///// </summary>
        ///// <param name="caller">The class context used to bind the callback.</param>
        ///// <param name="newHandler">The user callback called to handle an error.</param>
        ///// <param name="errorTypes">Error types to be handled by the handler.</param>
        ///// <returns>
        ///// The PHP representation of previous user handler, <B>null</B> if there is no user one, or 
        ///// <B>false</B> if <paramref name="newHandler"/> is invalid or empty.
        ///// </returns>
        ///// <remarks>
        ///// Stores old user handlers on the stack so that it is possible to 
        ///// go back to arbitrary previous user handler.
        ///// </remarks>
        //public static object set_error_handler(PHP.Core.Reflection.DTypeDesc caller, PhpCallback newHandler, int errorTypes)
        //{
        //    if (!PhpArgument.CheckCallback(newHandler, caller, "newHandler", 0, false)) return null;

        //    PhpCallback old_handler = Configuration.Local.ErrorControl.UserHandler;
        //    PhpError old_errors = Configuration.Local.ErrorControl.UserHandlerErrors;

        //    // previous handler was defined by user => store it into the stack:
        //    if (old_handler != null)
        //    {
        //        if (OldUserErrorHandlers == null)
        //        {
        //            OldUserErrorHandlers = new Stack(5);
        //            RequestContext.RequestEnd += new Action(ClearOldUserHandlers);
        //        }
        //        OldUserErrorHandlers.Push(new ErrorHandlerRecord(old_handler, old_errors));
        //    }

        //    // sets the current handler:
        //    Configuration.Local.ErrorControl.UserHandler = newHandler;
        //    Configuration.Local.ErrorControl.UserHandlerErrors = (PhpError)errorTypes;

        //    // returns the previous handler:
        //    return (old_handler != null) ? old_handler.ToPhpRepresentation() : null;
        //}

        ///// <summary>
        ///// Restores the previous user error handler if there was any.
        ///// </summary>
        //public static bool restore_error_handler()
        //{
        //    // if some user handlers has been stored in the stack then restore the top-most, otherwise set to null:
        //    if (OldUserErrorHandlers != null && OldUserErrorHandlers.Count > 0)
        //    {
        //        ErrorHandlerRecord record = (ErrorHandlerRecord)OldUserErrorHandlers.Pop();

        //        Configuration.Local.ErrorControl.UserHandler = record.ErrorHandler;
        //        Configuration.Local.ErrorControl.UserHandlerErrors = record.ErrorTypes;
        //    }
        //    else
        //    {
        //        Configuration.Local.ErrorControl.UserHandler = null;
        //        Configuration.Local.ErrorControl.UserHandlerErrors = (PhpError)PhpErrorSet.None;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// Sets user defined handler to handle exceptions.
        ///// </summary>
        ///// <param name="caller">The class context used to bind the callback.</param>
        ///// <param name="newHandler">The user callback called to handle an exceptions.</param>
        ///// <returns>
        ///// The PHP representation of previous user handler, <B>null</B> if there is no user one, or 
        ///// <B>false</B> if <paramref name="newHandler"/> is invalid or empty.
        ///// </returns>
        ///// <remarks>
        ///// Stores old user handlers on the stack so that it is possible to 
        ///// go back to arbitrary previous user handler.
        ///// </remarks>
        //public static object set_exception_handler(PHP.Core.Reflection.DTypeDesc caller, PhpCallback newHandler)
        //{
        //    if (!PhpArgument.CheckCallback(newHandler, caller, "newHandler", 0, false)) return null;

        //    PhpCallback old_handler = Configuration.Local.ErrorControl.UserExceptionHandler;

        //    // previous handler was defined by user => store it into the stack:
        //    if (old_handler != null)
        //    {
        //        if (OldUserExceptionHandlers == null)
        //        {
        //            OldUserExceptionHandlers = new Stack(5);
        //            RequestContext.RequestEnd += new Action(ClearOldUserHandlers);
        //        }
        //        OldUserExceptionHandlers.Push(old_handler);
        //    }

        //    // sets the current handler:
        //    Configuration.Local.ErrorControl.UserExceptionHandler = newHandler;

        //    // returns the previous handler:
        //    return (old_handler != null) ? old_handler.ToPhpRepresentation() : null;
        //}

        ///// <summary>
        ///// Restores the previous user error handler if there was any.
        ///// </summary>
        //public static bool restore_exception_handler()
        //{
        //    if (OldUserExceptionHandlers != null && OldUserExceptionHandlers.Count > 0)
        //        Configuration.Local.ErrorControl.UserExceptionHandler = (PhpCallback)OldUserExceptionHandlers.Pop();
        //    else
        //        Configuration.Local.ErrorControl.UserExceptionHandler = null;

        //    return true;
        //}

        #endregion
    }
}

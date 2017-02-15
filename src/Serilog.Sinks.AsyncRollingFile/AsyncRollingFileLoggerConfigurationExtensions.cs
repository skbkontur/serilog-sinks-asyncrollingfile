﻿// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.File;
using Serilog.Sinks.RollingFile;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.AsyncRollingFile() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class AsyncRollingFileLoggerConfigurationExtensions
    {
        const int DefaultRetainedFileCountLimit = 31; // A long month of logs
        const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024;
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Write log events to a series of files asynchronously. Each file will be named according to
        /// the date of the first log entry written to it. Only simple date-based rolling is
        /// currently supported.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="pathFormat">String describing the location of the log files,
        /// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
        /// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="fileSizeLimitBytes">The maximum size, in bytes, to which any single log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <param name="buffered">Indicates if flushing to the output file can be buffered or not. The default
        /// is false.</param>
        /// <param name="shared">Allow the log files to be shared by multiple processes. The default is false.</param>
        /// <param name="flushToDiskInterval">If provided, a full disk flush will be performed periodically at the specified interval.</param>
        /// <param name="queueSize">The size of the concurrent queue used to feed the background worker thread. If
        /// the thread is unable to process events quickly enough and the queue is filled, subsequent events will be
        /// dropped until room is made in the queue.</param> 
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <remarks>The file will be written using the UTF-8 encoding without a byte-order mark.</remarks>
        public static LoggerConfiguration AsyncRollingFile(
            this LoggerSinkConfiguration sinkConfiguration,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            LoggingLevelSwitch levelSwitch = null,
            bool buffered = false,
            bool shared = false,
            TimeSpan? flushToDiskInterval = null,
            int queueSize = 65536)
        {
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return AsyncRollingFile(sinkConfiguration, formatter, pathFormat, restrictedToMinimumLevel, fileSizeLimitBytes,
                retainedFileCountLimit, levelSwitch, buffered, shared, flushToDiskInterval, queueSize);
        }

        /// <summary>
        /// Write log events to a series of files asynchronously. Each file will be named according to
        /// the date of the first log entry written to it. Only simple date-based rolling is
        /// currently supported.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="formatter">Formatter to control how events are rendered into the file. To control
        /// plain text formatting, use the overload that accepts an output template instead.</param>
        /// <param name="pathFormat">String describing the location of the log files,
        /// with {Date} in the place of the file date. E.g. "Logs\myapp-{Date}.log" will result in log
        /// files such as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="fileSizeLimitBytes">The maximum size, in bytes, to which any single log file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB.</param>
        /// <param name="retainedFileCountLimit">The maximum number of log files that will be retained,
        /// including the current log file. For unlimited retention, pass null. The default is 31.</param>
        /// <param name="buffered">Indicates if flushing to the output file can be buffered or not. The default
        /// is false.</param>
        /// <param name="shared">Allow the log files to be shared by multiple processes. The default is false.</param>
        /// <param name="flushToDiskInterval">If provided, a full disk flush will be performed periodically at the specified interval.</param>
        /// <param name="queueSize">The size of the concurrent queue used to feed the background worker thread. If
        /// the thread is unable to process events quickly enough and the queue is filled, subsequent events will be
        /// dropped until room is made in the queue.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <remarks>The file will be written using the UTF-8 encoding without a byte-order mark.</remarks>
        public static LoggerConfiguration AsyncRollingFile(
            this LoggerSinkConfiguration sinkConfiguration,
            ITextFormatter formatter,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            LoggingLevelSwitch levelSwitch = null,
            bool buffered = false,
            bool shared = false,
            TimeSpan? flushToDiskInterval = null,
            int queueSize = 65536)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            if (shared && buffered)
                throw new ArgumentException("Buffered writes are not available when file sharing is enabled.", nameof(buffered));

            ILogEventSink sink = new RollingFileSink(pathFormat, formatter, fileSizeLimitBytes, retainedFileCountLimit, buffered: buffered, shared: shared);

            if (flushToDiskInterval.HasValue)
            {
                sink = new PeriodicFlushToDiskSink(sink, flushToDiskInterval.Value);
            }

            return sinkConfiguration.Async(c => c.Sink(sink, restrictedToMinimumLevel, levelSwitch), queueSize);
        }
    }
}
﻿using Microsoft.Extensions.DependencyInjection;
using Rnwood.Smtp4dev.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rnwood.Smtp4dev.Model
{
    public static class ServiceExtensions
    {
        public static void UseSmtp4dev(this IServiceCollection services)
        {
            SettingsStore settingsStore = new SettingsStore();
            services.AddInstance<ISettingsStore>(settingsStore);

            string dbDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Smtp4dev");
            Directory.CreateDirectory(dbDirectory);
            FileInfo dbFile = new FileInfo(Path.Combine(dbDirectory, "Smtp4dev.db"));

            MessageStore messageStore = new MessageStore(dbFile);
            services.AddInstance<IMessageStore>(messageStore);

            services.AddSingleton<ISmtp4devEngine, Smtp4devEngine>();
        }
    }
}
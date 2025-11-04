The console app will first get all of the cached Iswcs from the CacheIswcs container in Cosmos DB and save them to the CachedIswcs.txt file.

It will then use this data to query the database and get the uncached Iswcs. These will be saved to the UncachedIswcs.txt file.

Finally, it will cache the Iswcs in the UncachedIswcs.txt file. It will do it in batches. The batch size can be configured in appsettings.json.
The uncached Iswcs are ordered by popularity. The most popular Iswcs will be processed first. In this case, a popular work is determined by the number of unique agencies who have works under that Iswc.
It will keep track of the progress in the Checkpoint.txt file. If you need to stop the app, it can be resumed from the point it left off.
If you want to start over you can clear the Checkpoint.txt file.

Both the Cached and Uncached Iswcs will be saved in the database. Table names: dbo.CachedIswcs and dbo.UncachedIswcs.
Every time the app is run, if the tables already exist, they will be backed up under the same name with the current date appended to the name.

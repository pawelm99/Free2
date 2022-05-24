using Infrastructure.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace MongoDBMVC.Context
{
    public class ComputerContext
    {
        IMongoDatabase database;
        IGridFSBucket gridFS;

        public ComputerContext()
        {
            
            
            var connectionString = "mongodb://localhost:27017";
            var connection = new MongoUrlBuilder(connectionString);
            var client = new MongoClient(connectionString);
            database = client.GetDatabase(connection.DatabaseName="Computers");
            gridFS = new GridFSBucket(database);
        }

        public IMongoCollection<Computers> Computers
        {
            get => database.GetCollection<Computers>("Computers");

        }
        public async Task<IEnumerable<Computers>> GetComputersAsync(int? year, string name)
        {
            var builder = new FilterDefinitionBuilder<Computers>();
            var filter = builder.Empty;

            if(!string.IsNullOrWhiteSpace(name))
            {
                filter = filter & builder.Regex("Name", new BsonRegularExpression(name));

            }

            if ( year.HasValue)
            {
                filter = filter & builder.Eq("Year", year.Value);
            }

            if(year.HasValue)
            {
                filter = filter & builder.Eq("Year", year.Value);
            }

            return await Computers.Find(filter).ToListAsync();
        }

        public async Task<Computers> GetComputers(string id) => await Computers.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();

        public async Task Create(Computers c) => await Computers.InsertOneAsync(c);
        public async Task Update(Computers c) => await Computers.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(c.Id)), c);
        public async Task Remove(string id) => await Computers.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        public async Task<byte[]> GetImage(string id) => await gridFS.DownloadAsBytesAsync(new ObjectId(id));

        public async Task StoreImage(string id, Stream imageSteram,string imageName)
        {
            var computer = await GetComputers(id);
            if(computer.HasImage())
            {
                await gridFS.DeleteAsync(new ObjectId(computer.ImageId));

            }
            var imageId = await gridFS.UploadFromStreamAsync(imageName, imageSteram);
            computer.ImageId = imageId.ToString();

            var filter = Builders<Computers>.Filter.Eq("_id",new ObjectId(computer.Id));
            var update = Builders<Computers>.Update.Set("ImageId", computer.ImageId);

            await Computers.UpdateOneAsync(filter, update);
        }



    }
}

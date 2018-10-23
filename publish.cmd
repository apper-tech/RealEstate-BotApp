nuget restore
msbuild RealEstateBot.sln -p:DeployOnBuild=true -p:PublishProfile=realestate-bot-Web-Deploy.pubxml -p:Password=a3rfwQbQmTsiTGsRQCHmcZFH9E4kJxJZihkg5NTwwpsSsTY8XHem6ADDyjGi


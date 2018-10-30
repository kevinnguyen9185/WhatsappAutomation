#docker run -d -p 4444:4444 --name selenium-hub selenium/hub

#docker run -v <Host machine path>:/home/seluser -d --link selenium-hub:hub selenium/node-chrome

export SEL_SHARED_FOLDER=/tmp
export LITEDB_CONN=/tmp

#WebAPI
dotnet run --selfolder=/tmp
REM robocopy c:\git\youtubeDl C:\git\KittenSignalR\KittenSignalR\bin\Release\netcoreapp3.0\publish
docker build -t myimage -f Dockerfile.txt .
docker images
docker create myimage
docker run -it -p 5000:80 -v "H:\My Videos\Remote:/youtubeDLs" -v "H:\My Videos\Remote:/app/youtubeDLs" --restart unless-stopped -d myimage
REM docker run -it -p 5000:80 -d myimage
timeout 2
docker ps -a

ECHO --to open a command prompt, run `docker exec -it containername cmd`
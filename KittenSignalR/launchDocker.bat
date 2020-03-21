docker build -t myimage -f Dockerfile .
docker images
docker create myimage
docker run -it -p 5000:80 -v "H:\My Videos\Remote:/youtubeDLs" -d myimage
timeout 2
docker ps -a
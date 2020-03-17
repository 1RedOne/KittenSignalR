docker build -t myimage -f Dockerfile .
docker images
docker create myimage
docker run -it -p 5000:80 -d myimage
## Docker
This section references training from [Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/).

### Docker commands
Follow this nice [exercise](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/3-exercise-deploy-docker-image-locally). 

1. Pull an image  
   For eg:  
   `docker pull mcr.microsoft.com/dotnet/samples:aspnetapp`  
   When we pull an image, Docker stores it locally and makes it available for running it as containers.
3. View images  
   For eg:  
   `docker image list`
   | REPOSITORY | TAG | IMAGE ID | CREATED | SIZE |
   | --- | --- | --- | --- | --- |
   | mcr.microsoft.com/dotnet/samples | aspnetapp | 6e2737d83726 | 6 days ago | 263MB |

   Notice the repository name.
4. Run a docker container  
   For eg:
   ````
   // Remember the port mapping format as H:C (Host:Container). Host first!
   docker run -p 8080:80 -d mcr.microsoft.com/dotnet/samples:aspnetapp
   ````
   The command maps port 80 in the container to port 8080 on your computer. So if you visit the page `http://localhost:8080`, you can see the running web app.
5. View active containers with the `docker ps` command.  
   `ps` means "process status". It's a shortcut for `docker container ls`.  
   Use `a` flag if you want to view stopped containers as well.
   
   For eg:
   | CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES |
   | --- | --- | --- | --- | --- | --- | --- |
   | 57b9587583e3 | mcr.microsoft.com/dotnet/core/samples:aspnetapp | "dotnet aspnetapp.dll" | 42 seconds ago | Up 41 seconds | 0.0.0.0:8080->80/tcp | elegant_ramanujan |
   | d27071f3ca27 | mcr.microsoft.com/dotnet/core/samples:aspnetapp | "dotnet aspnetapp.dll" | 5 minutes ago | Up 5 minutes | 0.0.0.0:8081->80/tcp | youthful_heisenberg |
6. Stop a container  
   For eg:  
   `docker stop elegant_ramanujan`
7. Restart a stopped container  
   For eg:  
   `docker start elegant_ramanujan`
8. Remove a container  
   Typically once a container is stopped, it should also be removed. Removing a container cleans up any resources it leaves behind. Once you remove a container, any changes made within its image filesystem are permanently lost.
   
   For eg:  
   `docker rm elegant_ramanujan`

   You can't remove a container that's running, but you can force a container to be stopped and removed with the `-f` flag. Only use this if the app inside the container doesn't need to perform a graceful shutdown.
   
   For eg:  
   `docker container rm -f elegant_ramanujan`
9. Remove docker images  
   For eg:  
   `docker image rm mcr.microsoft.com/dotnet/core/samples:aspnetapp`  
   Containers running the image must be terminated before the image can be removed.

### Docker Commands cheat sheet
https://docs.docker.com/get-started/docker_cheatsheet.pdf

https://cheat-sheets.nicwortel.nl/docker-cheat-sheet.pdf

### Create custom image with a Dockerfile
To create a Docker image containing your application, you'll typically begin by identifying a **base image** to which you add files and configuration information.

A Dockerfile contains the steps for building a custom Docker image. Follow along this [guide at Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/5-exercise-create-custom-docker-image).

Also checkout this great blog at [docker.com](https://www.docker.com/blog/9-tips-for-containerizing-your-net-application/) for tips on creating a great image.

#### Step 1: Clone this sample MSLearn repo to your projects folder:
`Ashishs-MacBook-Pro:RiderProjects ashishkhanal$ git clone https://github.com/MicrosoftDocs/mslearn-hotel-reservation-system.git`

#### Step 2: Go into src folder
`Ashishs-MacBook-Pro:RiderProjects ashishkhanal$ cd mslearn-hotel-reservation-system/src`

#### Step 3: Create a Dockerfile
`Ashishs-MacBook-Pro:src ashishkhanal$ touch Dockerfile`

#### Step 4: Open Dockerfile in Vim
`Ashishs-MacBook-Pro:src ashishkhanal$ vim Dockerfile`

Hit `i` to get into insert (write) mode. Make the following changes:

````
FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /src

# The project files for the web app and the library project are copied to the /src folder in the container.
COPY ["/HotelReservationSystem/HotelReservationSystem.csproj", "HotelReservationSystem/"]
COPY ["/HotelReservationSystemTypes/HotelReservationSystemTypes.csproj", "HotelReservationSystemTypes/"]

# Download the dependencies required by these projects from NuGet.
RUN dotnet restore "HotelReservationSystem/HotelReservationSystem.csproj"

# Copy the source code for the web app to the container. First . comes from the build context, and second . represents container's /src folder. Then build the app. The dll files are written to the /app folder in the container.
COPY . .
WORKDIR "/src/HotelReservationSystem"
RUN dotnet build "HotelReservationSystem.csproj" -c Release -o /app

# dotnet publish command copies the executables for the website to a new folder and removes any interim files. The files in this folder can then be deployed to a website.
RUN dotnet publish "HotelReservationSystem.csproj" -c Release -o /app

EXPOSE 80
# Move to the /app folder containing the published version of the web app.
WORKDIR /app

# When the container runs it should execute the command dotnet HotelReservationSystem.dll
ENTRYPOINT ["dotnet", "HotelReservationSystem.dll"]

````
Explanation of commands used in `Dockerfile`:
| Command | Action |
| --- | ----------- |
| FROM | Downloads the specified image and **creates a new container** based on this image. |
| WORKDIR | Sets the current working directory in the container, used by the subsequent commands. |
| COPY | Copies files from the host computer to the container. The first argument (`.`) is a file or folder on the host computer. The second argument (`.`) specifies the name of the file or folder to act as the destination in the container. In this case, the destination is the current working directory (`/src`). |
| RUN | Executes a command in the container. Arguments to the RUN command are command-line commands. |
| EXPOSE | Creates a configuration in the new image that specifies which ports to open when the container runs. If the container is running a web app, it's common to EXPOSE port 80. |
| ENTRYPOINT | Specifies the operation the container should run when it starts. In this example, it runs the newly built app. You specify the command you want to run and each of its arguments as a string array. |

#### Step 5: Save and exit
Hit `Esc` key and type `:wq` to save (w as in write) and exit the editor.

#### Step 6: Build the image
This command builds the image and stores it locally.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker build -t reservationsystem:v1 .
````
The `docker build` command creates a new image by running a Dockerfile.  
`docker build` command **creates a container**, runs commands in it, then commits the changes **to a new image**.

`-t` flag specifies the name of the image to be created.
`.` provides the build context for the source files for the COPY command: the set of files on the host computer needed during the build process. So the first `.` in the COPY is `Ashishs-MacBook-Pro:src ashishkhanal$` which contains following files:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8b76c1fe-1071-4542-a076-304de49103a6">

#### Step 7: Run the image
Give the container a name as `reservations`.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker run -d -p 8080:80 reservationsystem:v1 --name reservations
````
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f8695714-7ccf-46b4-b28b-219313498d1c">

#### Step 8: Remove the image
After you're done, you can remove it:

`docker rm reservations`

### Deploy Docker Image to Azure Container Instance
Follow this [exercise](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/7-exercise-deploy-docker-image-to-container-instance).

#### Create Azure Container Registry
1. Create a resource group  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cd2add06-7826-414f-aff5-9f9b951fb9ab">
2. Create container registry  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6031ce23-3884-4d53-b05a-23315a788ca6">
3. In the resource menu, under Settings, select Access keys. The Access keys pane for your container registry appears.
4. Enable the Admin user access switch.  
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8cdcd70e-f32a-407c-a81d-d796a5f354c5">
5. Make a note of the Registry name, Login server, Username, and password.

#### Tag an Image
In short tags are used in order to identify an image. Also [read this](https://stackoverflow.com/q/46327455/8644294) for more info.

You push an image from your local computer to Azure Container Registry by using `docker push` command. Before you push an image, you must create an alias for the image that specifies the repository and tag, that the Azure Container Registry will create (if it doesn't already exist).

A repository in Azure Container Registry is a collection of related Docker images, differentiated by their tags. So when you push an image with a new tag, it's added to the specified repository in the registry.

Tag the current `reservationsystem` image with the name of our Azure Container Registry.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker tag reservationsystem:v1 myregistry5.azurecr.io/reservationsystem:latest
````
<img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/51e076ec-f41c-45f6-89a3-b67a1b688310">

Now check the images again:  
<img width="800" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/56000c96-c9d5-454e-95d5-3722269b9432">

#### Push an Image
1. Sign into Azure Container Registry using `docker login <login-server>`  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/58d34abc-3fc7-44bb-9af9-f05a9effc59b">

2. Upload an image using `docker push <registry-name>.azurecr.io/reservationsystem:latest`
   ````
   Ashishs-MacBook-Pro:src ashishkhanal$ docker push myregistry5.azurecr.io/reservationsystem:latest
   ````

Login Issues if you don't use admin user:
https://stackoverflow.com/q/65316558/8644294

#### Verify an Image
In the resource menu, under Services, select Repositories. The Repositories pane for your container registry appears.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/30f8b83b-352c-4d06-86c9-37147d4efd66">

#### Run an Image
1. Create `Container Instances` resource  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fdf548ad-044a-4661-af7b-f9fe8b8e8e7e">
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d30a49a5-d6ce-48fe-905d-041e92c75e86">
   
   On the Advanced tab, set the _Restart Policy_ to _Always_, and leave all the other settings as is.

   Hit 'Review + Create' -> 'Create'.
   
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2fe6c799-c71c-4102-9442-f34d3bb02b57">

2. Navigate to the URL: http://container-hotelsystem-eastus-dev.c7bbehfraef3epdq.eastus.azurecontainer.io/api/reservations/1
   
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4215e131-c6c5-4bfa-913c-738434279c69">

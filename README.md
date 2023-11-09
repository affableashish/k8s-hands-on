# k8s-hands-on
This repo contains notes from Docker and Kubernetes course at [Microsoft Learn](https://learn.microsoft.com/en-us/training/paths/intro-to-kubernetes-on-azure/) and also [Kubernetes course](https://youtu.be/X48VuDVv0do?si=T1xGQVUEa2ZdTatH) by TechWorld with Nana.

Also check out this [video](https://youtu.be/4ht22ReBjno?si=gBkC4jhCS2G9ZYd5) for ELI5 version of Kubernetes.

## Microservices
A variant of the service-oriented architecture (SOA) structural style - arranges an application as a collection of loosely coupled services.

In a microservices architecture, services are fine-grained and the protocols are lightweight.

### Features
1. Segregates functionality into smaller separate services each with a single responsibility.
2. Scales out by deploying each service independently.
3. Makes the services loosely coupled.
4. Enable autonomouse development by different teams, languages and platforms.
5. Can be written by smaller teams.
6. Each Microsoervice can own its own data/database.

### Benefits
1. Improved fault isolation.
2. Eliminate vendor or technology lock in because it's based on open source tools.
3. Ease of understanding because a microservice's domain is small.
4. Smaller and faster deployments.
5. Scalability.

### Challenges
Complexity increases by quite a bit.

1. Testing.
2. Deployment. One Microservice can impact many microservices.
3. Manage multiple databases.
4. Latency issues.
5. Transient errors.
6. Multiple point of failures.
7. Complex security.

### Monolith to Microservices migration
Incrementally migrate a legacy system by gradually replacing specific pieces of functionality with new applications and services. As features from the legacy system are replaced, the new system eventually replaces all of the old system's features, strangling the old system and allowing you to decommission it.

<img width="800" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e0de73ca-d817-4c80-b375-b0dfb5c7d47d">

[Reference](https://learn.microsoft.com/en-us/azure/architecture/patterns/strangler-fig)

## Cloud Native
Cloud native technologies empower organizations to build and run scalable applications in the cloud.
1. Uses containers, service meshes, microservices, immutable infrastructure and declarative APIs.
2. Enable loosely coupled systems that are resilient, manageable and observable. Combined with automation, they allow to make changes frequently.
3. Uses ecosystem of open source, vendor neutral projects.

View the Cloud Native Landscape here: https://landscape.cncf.io/

### Cloud Native Concepts
1. Speed and Agility
2. Application Architecture  
   <img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/dc5b9041-9838-4010-893a-4125e32c7892">
3. Mentality : Pets vs Cattle
   1. Infrastructure becomes like cattle, as it becomes immutable and disposable.
   2. Provisioned in minutes and destroyed on demand.
   3. Never updated or repaired, but re-provisioned.
4. Greenfield vs Brownfield
   Cloud native projects are a lot easier with New projects but also possible with Legacy projects.
5. Cloud Native Trail Map
   Breaks the journey into smaller measurable objectives.  
   Diagram here: https://raw.githubusercontent.com/cncf/trailmap/master/CNCF_TrailMap_latest.png  
   Taken from [here](https://github.com/cncf/trailmap).

## Containers
Finish the [Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/) training. It's pretty great.

Suppose you work for an online clothing retailer that's planning the development of several internal apps. Your team develops and tests all applications on-premises and then deploys them to Azure for pre-production testing and final production hosting. You're looking for maximum compatibility in each environment with little or no configuration changes. Using Docker as a containerization solution seems an ideal choice.

A **container** is a loosely isolated environment that allows us to build and run software packages. These software packages include the code and all dependencies (for eg: operating system, runtime, system tools, system libraries and so on) to run applications quickly and reliably on any computing environment. We call these packages container **images**. Docker containers are built off of Docker images. **The container is the in-memory instance of an image.** Since images are readonly, Docker adds a read-write file system over the read-only file system of the image to create a container.

<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/42ed1fcf-becf-411f-bc88-ddf17f8e0f54">

### Container Image
A container image is a portable package that contains software. It's this image that, when run, becomes our container. 

A container image is immutable. Once you've built an image, you can't change it. The only way to change an image is to create a new image. This feature is our guarantee that the image we use in production is the same image used in development and QA.

[Free Code Camp reference](https://www.freecodecamp.org/news/a-beginner-friendly-introduction-to-containers-vms-and-docker-79a9e3e119b/).

[Microsoft Learn reference](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/).

### VMs vs Containers
<img width="480" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8731f9bc-291e-425d-b328-8d78103a07d7">

### Container Registry
Docker images are stored and made available in registries. A registry is a web service to which Docker can connect to upload and download container images. When you download and run an image, you must specify the registry, repository and version tag for the image.

For eg:
````
mcr.microsoft.com/dotnet/core/aspnet:7   <-- Tag is 7
mcr.microsoft.com/dotnet/core/aspnet:8
````

| Term | Value |
| --- | ----------- |
| Registry | mcr.microsoft.com |
| Repository | dotnet/core/aspnet |
| Image Name | aspnet |
| Version Tag | 7 |

The repository name must be of the form `*<login_server>/<image_name>:<tag/>`.

For eg: This is how repositories looks like:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/29cbde5c-6ad8-4ef6-8961-4f571f9414b7">

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/13325509-40fd-4b43-9b9f-f13e65b0f382">

Also take a look at [Microsoft Artifact Registry](https://mcr.microsoft.com/en-us/product/dotnet/samples/tags):

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1d5335a5-a36d-4ded-9802-7d2dba8e2eaf">

### How Image is created
Go to this site:
https://hub.docker.com/_/microsoft-dotnet-samples/

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bd9dfe43-2529-4cc3-a5d6-6eefaddcc3de">

And check out the Dockerfile:

https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/Dockerfile

````
# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY aspnetapp/*.csproj .
RUN dotnet restore -a $TARGETARCH

# copy and publish app and libraries
COPY aspnetapp/. .
RUN dotnet publish -a $TARGETARCH --no-restore -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./aspnetapp"]
````

### Container and files
If a running container makes changes to the files in its image, those changes only exist in the container where the changes are made. Unless you take specific steps to preserve the state of a container, these changes are lost when the container is removed. Similarly, multiple containers based on the same image that run simultaneously don't share the files in the image. Each container has its own independent copy. Any data written by one container to its filesystem isn't visible to the other.

It's a best practice to avoid the need to make changes to the image filesystem for applications deployed with Docker. Only use it for temporary files that can afford to be lost.

It's possible to add writable volumes to a container. A volume represents a filesystem that can be mounted by the container, and is made available to the application running in the container. The data in a volume does persist when the container stops, and multiple containers can share the same volume.

Recommended Reading: https://stackoverflow.com/a/47152658/8644294.

## Docker
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

| Command | Action |
| --- | ----------- |
| FROM | Downloads the specified image and **creates a new container** based on this image. |
| WORKDIR | Sets the current working directory in the container, used by the subsequent commands. |
| COPY | Copies files from the host computer to the container. The first argument (`.`) is a file or folder on the host computer. The second argument (`.`) specifies the name of the file or folder to act as the destination in the container. In this case, the destination is the current working directory (`/src`). |
| RUN | Executes a command in the container. Arguments to the RUN command are command-line commands. |
| EXPOSE | Creates a configuration in the new image that specifies which ports to open when the container runs. If the container is running a web app, it's common to EXPOSE port 80. |
| ENTRYPOINT | Specifies the operation the container should run when it starts. In this example, it runs the newly built app. You specify the command you want to run and each of its arguments as a string array. |

The `docker build` command creates a new image by running a Dockerfile.
`docker build` command **creates a container**, runs commands in it, then commits the changes to a new image.

#### Step 5: Save and exit
Hit `Esc` key and type `:` then type `wq` to write and exit the editor.

#### Step 6: Build the image
This command builds the image and stores it locally.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker build -t reservationsystem:v1 .
````
`-t` flag specifies the name of the image to be created.
`.` provides the build context for the source files for the COPY command: the set of files on the host computer needed during the build process. So the first `.` in the COPY is `Ashishs-MacBook-Pro:src ashishkhanal$` which contains following files:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8b76c1fe-1071-4542-a076-304de49103a6">

#### Step 7: Run the image
Give the container a name as `reservations`.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker run -d -p 8080:80 reservationsystem:v1 --name reservations
````
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f8695714-7ccf-46b4-b28b-219313498d1c">

After you're done, remove it: `docker rm reservations`

### Deploy Docker Image to Azure Container Instance
Follow this [exercise](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/7-exercise-deploy-docker-image-to-container-instance).

#### Create Azure Container Registry
1. Create a resource group
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cd2add06-7826-414f-aff5-9f9b951fb9ab">

2. Create container registry
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6031ce23-3884-4d53-b05a-23315a788ca6">

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
<img width="850" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/51e076ec-f41c-45f6-89a3-b67a1b688310">

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
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fdf548ad-044a-4661-af7b-f9fe8b8e8e7e">
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d30a49a5-d6ce-48fe-905d-041e92c75e86">
   
   On the Advanced tab, set the _Restart Policy_ to _Always_, and leave all the other settings as is.

   Hit 'Review + Create' -> 'Create'.
   
   <img width="850" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2fe6c799-c71c-4102-9442-f34d3bb02b57">

2. Navigate to the URL: http://container-hotelsystem-eastus-dev.c7bbehfraef3epdq.eastus.azurecontainer.io/api/reservations/1
   
   <img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4215e131-c6c5-4bfa-913c-738434279c69">

## Intro to Kubernetes
Reference: https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/

Kubernetes is a portable, extensible open source platform for management and orchestration of containerized workloads.

### Container management
It is the process of organizing, adding, removing or updating a significant number of containers.

An app has an environment that it relies upon to run.  
For eg:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/7bd896d9-78c7-4805-9ea7-e82f384a857e">

Container provides an isolated context in which an app together with its environment can run but those isolated containers often need to be managed and connected to the external world. Shared file systems, networking, scheduling, load balancing and distribution are all challanges.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/871aae6b-1a1e-4907-8b2d-e33699181305">

[Reference](https://youtu.be/4ht22ReBjno?si=gBkC4jhCS2G9ZYd5).

### Container Orchestrator
It is a system that automatically deploys and manages containerized apps. As part of management, it handles scaling dynamic changes in the environment to increase or decrease the number of deployed instances of the app. 
It also ensures that all deployed container instances are updated when a new version of a service is released.

<img width="750" alt="image" src="https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/2-tasks-of-orchestrator.svg">

### Kubernetes Benefits
<img width="650" alt="image" src="https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/2-kubernetes-benefits.svg">

These tasks include:
1. Self-healing of containers; for example, restarting containers that fail or replacing containers
2. Scaling deployed container count up or down dynamically, based on demand
3. Automating rolling updates and rollbacks of containers
4. Managing storage
5. Managing network traffic
6. Storing and managing sensitive information such as usernames and passwords

### Kubernetes Considerations
With Kubernetes, you can view your datacenter as one large compute resource. You don't need to worry about how and where you deploy your containers, only about deploying and scaling your apps as needed.

<img width="650" alt="image" src="https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/2-kubernetes-considerations.svg">

However, it's important to understand that Kubernetes isn't a single installed app that comes with all possible components needed to manage and orchestrate a containerized solution:  
1. Aspects such as deployment, scaling, load balancing, logging, and monitoring are all optional. You're responsible for finding the best solution that fits your needs to address these aspects.
2. Kubernetes doesn't limit the types of apps that can run on the platform. If your app can run in a container, it can run on Kubernetes. To make optimal use of containerized solutions, your developers need to understand concepts such as microservices architecture.
3. Kubernetes doesn't provide middleware, data-processing frameworks, databases, caches, or cluster-storage systems. All these items are run as containers, or as part of another service offering.
4. For Kubernetes to run containers, it needs a container runtime like Docker or containers. The container runtime is the object that's responsible for managing containers. For example, the container runtime starts, stops, and reports on the container's status.
5. You're responsible for maintaining your Kubernetes environment. For example, you need to manage OS upgrades and the Kubernetes installation and upgrades. You also manage the hardware configuration of the host machines, such as networking, memory, and storage.

Cloud services such as Azure Kubernetes Service (AKS) reduce these challenges by providing a hosted Kubernetes environment.

### How Kubernetes Works
<img width="950" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/10663c92-689f-43f3-91e3-40e840ed2132">

#### Cluster
Cluster is a set of computers that you configure to work together and view as a single system. The computers configured in the cluster handle same kind of tasks. For eg, they all host websites, APIs or run compute intensive works.

A cluster uses centralized software that's responsible for scheduling and controlling these tasks.  
The computers in a cluster that run the tasks are called nodes, and the computers that run the scheduling software are called control planes.  
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4e3a1c69-51da-4695-9c12-a2845f02f168">

#### Kubernetes Architecture
You use Kubernetes as the orchestration and cluster software to deploy your apps and respond to changes in compute resource needs.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e1202f9e-e735-42db-974a-173445306e7c">

A K8s cluster contains at least one main plane and one or more nodes. THe default host OS in K8s is Linux, with default support for Linux based workloads.<br>
You can also run Microsoft workloads by using Windows Server 2019 or later on cluster nodes. For eg: if you have some app that's written as .NET 4.5, this can run only on nodes that run a Windows Server OS.

#### Kubernetes Node
A node in a K8s cluster is where your compute workloads run. Each node communicates with the control plane via the **API server** to inform it about state changes on the node.

#### Kubernetes Control Plane
Kubernetes control plane runs a collection of services that manage the orchestration functionality in K8s.  
It is responsible for maintaining the desired state of the cluster, such as which applications are running and which container images they use. These apps are user deployed apps and not services running the control plane like API server, Scheduler etc. (Nodes actually run the applications and workloads).

K8s relies on several administrative services running on the control plane. These services manage aspects such as cluster-component communication, workload scheduling, and cluster-state persistence.

The following services make up a Kubernetes cluster's control plane:
1. API server
2. Backing store
3. Scheduler
4. Controller manager
5. Cloud controller manager

Test environment -> 1 control plane<br>
Prod environment -> 3-5 controls planes

The fact that a control plane runs specific software to maintain the state of the cluster doesn't exclude it from running other compute workloads. However, you usually want to exclude the control plane from running noncritical and user app workloads.

#### API server
You can think of API server as the frontend to your K8s control plane. All the communication between the components in K8s is done through this API.  
For eg, as a user you use a command line app called `kubectl` that allows you to run commands against your K8s cluster's API server.
The component that provides this API is called `kube-apiserver`, and you can deploy several instances of this component to support scaling in your cluster.  
This API exposes a RESTful API that you can use to post commands or YAML-based configuration files. You use YAML files to define the intended state of all the objects within a Kubernetes cluster.  
For example, assume that you want to increase the number of instances of your app in the cluster. You define the new state with a YAML-based file and submit this file to the API server. The API server validates the configuration, save it to the cluster, and finally enact the configured increase in app deployments.

#### Backing store
The backing store is a persistent storage that your Kubernetes cluster saves it's complete configuration inside. Kubernetes uses a high-availability, distributed, and reliable key-value store called `etcd`. This key-value store stores the current state and the desired state of all objects within your cluster.

In a production Kubernetes cluster, the official Kubernetes guidance is to have three to five replicated instances of the `etcd` database for high availability.

`etcd` isn't responsible for data backup. It's your responsibility to ensure that an effective backup plan is in place to back up the `etcd` data.

#### Scheduler
The scheduler is the component that's responsible for the assignment of workloads across all nodes. The scheduler monitors the cluster for newly created containers and assigns them to nodes.

For eg:  
The Kubernetes scheduler does not create new instances of containers. Instead, it’s responsible for determining on which node in the Kubernetes cluster a new pod (which can contain one or more containers) should run.  
Here’s a simplified version of how it works:
1. A user or a controller creates a new pod by submitting it to the Kubernetes API server.
2. The API server adds the new pod to its store of Kubernetes objects and makes it available to the scheduler.
3. The scheduler watches for newly created pods that have no node assigned. This is what is meant by “monitoring newly created containers”.
4. For each new pod, the scheduler determines which nodes have enough free resources to run the pod.
5. The scheduler then ranks each suitable node based on its scheduling algorithm and assigns the pod to the best node.
6. Once the scheduler has made its decision, it notifies the API server, which in turn notifies the chosen node.
   
So, the scheduler doesn’t create containers or pods. It simply decides where new pods should run based on the current state of the cluster and the resource requirements of the new pod. The actual creation and management of containers within a pod is handled by the Kubelet on the chosen node.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5304b47a-6e3d-42ab-b692-17fe30a55308">

#### Controller manager
Controller manager launches and monitors the controllers configured for a cluster through the API server. "through the API server" means that the controller manager uses the API server to interact with the K8s objects that the controllers manage.  

1. **Track Object States:**  
   Kubernetes uses controllers to track object states in the cluster. Each controller runs in a non-terminating loop, watching and responding to events in the cluster. For example, there are controllers to monitor nodes, containers, and endpoints.

2. **Communicate with the API Server:**  
   The Controller communicates with the API server to determine the object's state. It checks if the current state of an object is different from its desired state.

3. **Ensure Desired State:**  
   If the current state of an object is different from its desired state, the controller takes action to ensure the desired state.  
   Suppose that one of three containers running in your cluster stops responding and has died. In this case, a controller decides whether you need to launch new containers to ensure that your apps are always available. If the desired state is to run three containers at any time, then a new container is scheduled to run.

#### Cloud controller manager
The cloud controller manager integrates with the underlying cloud technologies in your cluster when the cluster is running in a cloud environment. These services can be load balancers, queues, and storage, for example.

1. Suppose you have a service in your Kubernetes cluster that needs to be exposed to the internet. You would define this service in Kubernetes and specify that it needs a load balancer.
2. The CCM sees this new service and communicates with Azure to create a new Azure Load Balancer.
3. The CCM then configures the Azure Load Balancer with the necessary settings, such as which ports to listen on and which pods to forward traffic to.
4. The CCM updates the service in Kubernetes with the details of the new Azure Load Balancer, such as its IP address.

So CCM is essentially a bridge between your K8s cluster and your cloud provider.

#### Services that run on a node
There are several services that run on a K8s node to control how workloads run.

<img width="360" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0d043a3f-019c-46b8-9a97-f947207a8f9c">

The following services run on the Kubernetes node:

1. **Kubelet**
2. **Kube-proxy**
3. **Container runtime**

#### Kubelet
Agent that runs on each node in the cluster and monitors work requests from the API server. It makes sure that the requested unit of work is running and healthy.

It monitors the nodes and makes sure that the containers scheduled on each node run as expected. It manages only containers K8s creates. It isn't responsible for rescheduling work to run on other nodes if the current node can't run the work.

#### Kube-proxy
It is responsible for local cluster networking, and runs on each node. It ensures that each node has a unique IP address. It also implements rules to handle routing and load balancing of traffic by using iptables and IPVS.

This proxy doesn't provide DNS services by itself. A DNS cluster add-on based on CoreDNS is recommended and installed by default.

#### Container runtime
Container runtime is the underlying software that runs containers on a K8s cluster.
It's responsible for fetching, starting and stopping container images. It supports several container runtimes, like: Docker, containerd, rkt, CRI-O frakti etc.

The support for many container runtime types is based on the Container Runtime Interface (CRI). The CRI is a plug-in design that enables the kubelet to communicate with the available container runtime.

The default container runtime in AKS is `containerd`, an industry-standard container runtime.

#### Interact with a K8s cluster
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a2adfddb-e6f5-4bac-ac23-540d381b32a7">

K8s provides a command line tool called `kubectl` to manage your cluster. You use `kubectl` to send commands to the cluster's control plane or fetch information about all K8s objects via the API server.

`kubectl` uses a configuration file that includes the following configuration information:

1. **Cluster** configuration specifies a cluster name, certificate information, and the service API endpoint associated with the cluster. This definition allows you to connect from a single workstation to multiple clusters.
2. **User** configuration specifies the users and their permission levels when they're accessing the configured clusters.
3. **Context** configuration groups clusters and users by using a friendly name. For example, you might have a "dev-cluster" and a "prod-cluster" to identify your development and production clusters.

You can configure `kubectl` to connect to multiple clusters by providing the correct context as part of the command-line syntax.

#### Kubernetes Pods
A pod represents a single instance of an app running in K8s. 

<img width="424" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c7016ccb-02df-4396-afa1-1c53cd665787">

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cad0406a-8b59-4282-b58f-1c973ddd4884">

Unlike in a Docker environment, you can't run containers directly on Kubernetes. You package the container into a Kubernetes object called a pod. A pod is the smallest object that you can create in Kubernetes.

A single pod can hold a group of one or more containers. However, a pod typically doesn't contain multiples of the same app.

A pod includes information about the shared storage and network configuration and a specification about how to run its packaged containers. You use pod templates to define the information about the pods that run in your cluster. Pod templates are YAML-coded files that you reuse and include in other objects to manage pod deployments.

For example, let's say that you want to deploy a website to a Kubernetes cluster. You create the pod definition file that specifies the app's container images and configuration. Next, you deploy the pod definition file to Kubernetes.

Assume that your site uses a database. The website is packaged in the main container, and the database is packaged in the supporting container. Multiple containers communicate with each other through an environment. The containers include services for a host OS, network stack, kernel namespace, shared memory, and storage volume. The pod is the sandbox environment that provides all of these services to your app. The pod also allows the containers to share its assigned IP address.

Because you can potentially create many pods that are running on many nodes, it can be hard to identify them. You can recognize and group pods by using string labels that you specify when you define a pod.

#### Lifecycle of Kubernetes Pods
Kubernetes pods have a distinct lifecycle that affects the way you deploy, run, and update pods. You start by submitting the pod YAML manifest to the cluster. After the manifest file is submitted and persisted to the cluster, it defines the desired state of the pod. The scheduler schedules the pod to a healthy node that has enough resources to run the pod.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/34330559-426b-433f-9790-81c8b0b150e9">

| Phase | Description |
| --- | ----------- |
| Pending | The cluster has accepted the pod, but not all containers are set up or ready to run. The Pending status indicates the time a pod is waiting to be scheduled and the time spent downloading container images. |
| Running | The pod transitions to a running state after all of the resources within the pod are ready. |
| Succeeded | The pod transitions to a succeeded state after the pod completes its intended task and runs successfully. |
| Failed | Pods can fail for various reasons. A container in the pod might have failed, leading to the termination of all other containers, or maybe an image wasn't found during preparation of the pod containers. In these types of cases, the pod can go to a Failed state. Pods can transition to a failed state from either a Pending state or a Running state. A specific failure can also place a pod back in the pending state. |
| Unknown | If the state of the pod can't be determined, the pod is an Unknown state. |

Pods are kept on a cluster until a controller, the control plane, or a user explicitly removes them. When a pod is deleted, a new pod is created immediately after. The new pod is considered an entirely new instance based on the pod manifest.

The cluster doesn't save the pod's state or dynamically assigned configuration. For example, it doesn't save the pod's ID or IP address. This aspect affects how you deploy pods and how you design your apps. For example, you can't rely on preassigned IP addresses for your pods.

#### Container states
Keep in mind that the phases explained above are a summary of where the pod is in its lifecycle. When you inspect a pod, the cluster uses three states to track your containers inside the pod:

| State | Description |
| --- | ----------- |
| Waiting | Default state of a container and the state that the container is in when it's not running or terminated. |
| Running | The container is running as expected without any problems. |
| Terminated | The container is no longer running. The reason is that either all tasks finished or the container failed for some reason. A reason and exit code are available for debugging both cases. |

### How Kubernetes deployments work
The drone-tracking app has several components that are deployed separately from each other. It's your job to configure deployments for these components on the cluster.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c955a7b0-f0ea-45e6-b031-d548e70ea8cb">

#### Pod Deployment options
There are several options to manage the deployment of pods in a Kubernetes cluster when you're using `kubectl`. The options are:

1. **Pod templates**
2. **Replication Controllers**
3. **Replica set**
4. **Deployments**

You can use any of these four Kubernetes object-type definitions to deploy a pod or pods. These files make use of YAML to describe the intended state of the pod or pods to be deployed.

#### Pod Template
A pod template enables you to define the configuration of the pod you want to deploy. The template contains information such as the name of container image and which container registry to use to fetch the images. The template also includes runtime configuration information, such as ports to use. Templates are defined by using YAML in the same way as when you create Docker files.

You can use templates to deploy pods manually. However, a manually deployed pod isn't relaunched after it fails, is deleted, or is terminated. To manage the lifecycle of a pod, you need to create a higher-level Kubernetes object.

#### Replication Controller
A replication controller uses pod templates and defines a specified number of pods that must run. The controller helps you run multiple instances of the same pod, and ensures pods are always running on one or more nodes in the cluster. The controller replaces running pods in this way with new pods if they fail, are deleted, or are terminated.

For example, assume you deploy the drone tracking front-end website, and users start accessing the website. If all the pods fail for any reason, the website is unavailable to your users unless you launch new pods. A replication controller helps you make sure your website is always available.

#### Replica Set
A replica set replaces the replication controller as the preferred way to deploy replicas. A replica set includes the same functionality as a replication controller, but it has an extra configuration option to include a selector value.

A selector enables the replica set to identify all the pods running underneath it. Using this feature, you can manage pods labeled with the same value as the selector value, but not created with the replicated set.

#### Deployment (abstraction over Pods)
A deployment creates a management object one level higher than a replica set, and allows you to deploy and manage updates for pods in a cluster.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0a5337f0-ee99-49b4-b565-6c296c3f1e2e">

Databases can't be replicated via deployment because Db have state. If we have clones or replica of the database, they would all need to access the same shared data storage and they you would need some kind of mechanism that manages which pods are currently reading/ writing from/ to that storage in order to avoid data inconsistencies and that mechanism in addition to replicating feature is offered by another K8s component called **StatefulSet**. This component is meant specifically for databases.

Deploying StatefulSet is not easy that's why Databases are often hosted outside the K8s cluster.

<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/3b5c22f4-fc5a-4e9b-b30e-52f67ace66fc">

**Layers of Abstraction:**  
Everything below Deployment is handled by K8s.  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bad2acfa-45da-4a5a-aea5-aa458da21337">

Eg:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ed9b483e-c722-461d-a89a-c0829c58dac1">

Assume that you have five instances of your app deployed in your cluster. There are five pods running version 1.0.0 of your app.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/80697bac-8828-40bb-8ac5-e05b9f52f3ff">

If you decide to update your app manually, you can remove all pods, then launch new pods running version 2.0.0 of your app. With this strategy, your app experiences downtime.

Instead, you want to execute a rolling update where you launch pods with the new version of your app before you remove the older app versioned pods. Rolling updates launch one pod at a time instead of taking down all the older pods at once. Deployments honor the number of replicas configured in the section that describes information about replica sets. It maintains the number of pods specified in the replica set as it replaces old pods with new pods.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/3b91f482-730f-4279-8740-05975b9d3b0d">

Deployments, by default, provide a rolling update strategy for updating pods. You can also use a re-create strategy. This strategy terminates pods before launching new pods.

Deployments also provide you with a rollback strategy, which you can execute by using `kubectl`.

Deployments make use of YAML-based definition files and make it easy to manage deployments. Keep in mind that deployments allow you to apply any changes to your cluster. For example, you can deploy new versions of an app, update labels, and run other replicas of your pods.

`kubectl` has convenient syntax to create a deployment automatically when you're using the `kubectl run` command to deploy a pod. This command creates a deployment with the required replica set and pods. However, the command doesn't create a definition file. It's a best practice to manage all deployments with deployment definition files, and track changes by using a version-control system.

#### Deployment Considerations
Kubernetes has specific requirements about how you configure networking and storage for a cluster. How you configure these two aspects affects your decisions about how to expose your apps on the cluster network and store data.

For example, each of the services in the drone-tracking app has specific requirements for user access, inter-process network access, and data storage. Now, take a look at these aspects of a Kubernetes cluster and how they affect the deployment of apps.

#### Kubernetes networking
Assume you have a cluster with one control plane and two nodes. When you add nodes to Kubernetes, an IP address is automatically assigned to each node from an internal private network range. For example, assume that your local network range is `192.168.1.0/24`.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a3894369-3f70-4240-9346-1e88d7771a3f">

Each pod that you deploy gets assigned an IP from a pool of IP addresses. For example, assume that your configuration uses the 10.32.0.0/12 network range, as the following image shows.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bb814554-897a-457e-9c8d-4d959afe57b9">

By default, the pods and nodes can't communicate with each other by using different IP address ranges.

To further complicate matters, recall that pods are transient. The pod's IP address is temporary, and can't be used to reconnect to a newly created pod. This configuration affects how your app communicates to its internal components and how you and services interact with it externally.

To simplify communication, Kubernetes expects you to configure networking in such a way that:

1. Pods can communicate with one another across nodes without Network Address Translation (NAT).
2. Nodes can communicate with all pods, and vice versa, without NAT.
3. Agents on a node can communicate with all nodes and pods.

Kubernetes offers several networking options that you can install to configure networking. Examples include Antrea, Cisco Application Centric Infrastructure (ACI), Cilium, Flannel, Kubenet, VMware NSX-T, and Weave Net.

Cloud providers also provide their own networking solutions. For example, Azure Kubernetes Service (AKS) supports the Azure Virtual Network container network interface (CNI), Kubenet, Flannel, Cilium, and Antrea.

---

##### Explanation on `192.168.1.0/24`
Imagine you live in a big apartment building. This building is like your network. Each apartment in the building is like an IP address - it’s a specific location in the network.

The building has 32 floors (8bits.8bits.8bits.8bits) but 24 floors are used to identify the building (network) itself. The remaining 8 floors (bits) are used for the apartments (hosts) within that building (network). 2^8 = 256 IP addresses.

In Computer Networks language, `192.168.1.0/24` represents a subnet with a netmask of `255.255.255.0`. This means that the subnet can provide up 256 IP addresses, ranging from `192.168.1.0` to `192.168.1.255`.  
However, in practice, the first address of a subnet (192.168.1.0 in this case) is reserved for the network address, and the last address (192.168.1.255) is reserved for the broadcast address. Therefore, there are typically 254 usable IP addresses (192.168.1.1 through 192.168.1.254) for hosts in this subnet.

##### Explanation on `10.32.0.0/12`
Keep 12 bits to identify the network and use 20 bits for the hosts.  
So first octet (8 bits) = 10. We want to figure out last 4 bits of the second octet because that's what's used for the hosts.  
2^4 = 16, so that's what's needed to be added to `.32` to find max in second octet. 32 + 15 (0-15 is 16) = 47.  
So the IP range is: `10.32.0.0` to `10.47.255.255`.

#### Kubernetes services
A K8s service is a K8s object that provides stable networking for pods. A Kubernetes service enables communication between nodes, pods, and users of your app, both internal and external, to the cluster.

Service also provides load balancing.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/73075614-7b57-415d-85ab-73f0e0be0833">

To make your app accessible through the browser, you have to create an external service. External service opens communication from external sources into your cluster.

#### Kubernetes Ingress
The request goes to the Ingress which forwards it to the Service.
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/12cc8886-4592-41f3-a543-280e286fafbc">

#### Group Pods
Managing pods by IP address isn't practical. Pod IP addresses change as controllers re-create them, and you might have any number of pods running.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d200b5cc-9adf-4472-9389-07bbd55323ef">

A service object allows you to target and manage specific pods in your cluster by using selector labels. You set the selector label in a service definition to match the pod label defined in the pod's definition file.

For example, assume that you have many running pods. Only a few of these pods are on the front end, and you want to set a LoadBalancer service that targets only the front-end pods. You can apply your service to expose these pods by referencing the pod label as a selector value in the service's definition file. The service groups only the pods that match the label. If a pod is removed and re-created, the new pod is automatically added to the service group through its matching label.

#### ConfigMap
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f1f1f3c4-285e-4d01-9996-0a9412a2c2f3">

#### Secrets
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f355abc5-222e-4ff8-9ebe-f1c80a230b43">

#### Kubernetes storage
Kubernetes uses the same storage volume concept that you find when using Docker. Docker volumes are less managed than the Kubernetes volumes, because Docker volume lifetimes aren't managed. The Kubernetes volume's lifetime is an explicit lifetime that matches the pod's lifetime. This lifetime match means a volume outlives the containers that run in the pod. However, if the pod is removed, so is the volume.

<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1bcdf8e2-dbe4-4775-8381-943fb1932eef">

Kubernetes provides options to provision persistent storage with the use of PersistentVolumes. You can also request specific storage for pods by using PersistentVolumeClaims.

Keep both of these options in mind when you're deploying app components that require persisted storage, like message queues and databases.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0b2bf7c7-d4e3-4c4f-868b-a8fe89b4c3fa">

#### Cloud integration considerations
Kubernetes doesn't provide any of the following services:

1. **Middleware**
2. **Data-processing frameworks**
3. **Databases**
4. **Caches**
5. **Cluster storage systems**

In this drone-tracking solution, there are three services that provide middleware functionality: a NoSQL database, an in-memory cache service, and a message queue.

When you're using a cloud environment such as Azure, it's a best practice to use services outside the Kubernetes cluster. This decision can simplify the cluster's configuration and management. For example, you can use Azure Cache for Redis for the in-memory caching services, Azure Service Bus messaging for the message queue, and Azure Cosmos DB for the NoSQL database.

### Explore K8s cluster 
Take a look at [this](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/5-exercise-kubernetes-functionality?pivots=macos).

#### Install MicroK8s
To run MicroK8s on macOS, use Multipass. Multipass is a lightweight VM manager for Linux, Windows, and macOS.

1. `brew install --cask multipass`  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0d0cc10e-02c3-4dc2-99c1-ff9bf2cc2438">

2. Run the multipass launch command to configure and run the microk8s-vm image.  
   `multipass launch --name microk8s-vm --memory 4G --disk 40G`  
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d934b92e-55dd-4385-9eb2-8fd0e9b27882">

3. Enter into the VM instance  
   `multipass shell microk8s-vm`  
   <img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f1ea5e32-1096-49ea-b61f-17d02880df03">
   At this point, you can access the Ubuntu VM to host your cluster. You still have to install MicroK8s.  

4. Install the MicroK8s snap app.  
   `sudo snap install microk8s --classic`  
   <img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6ae3c2ca-5c8f-42c3-a398-04991edf4220">

#### Prepare the cluster
1. Check the status of the installation  
   `sudo microk8s.status --wait-ready`  
   <img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6fdf15b9-11de-4457-86af-0f92613f3105">

2. Enable the 3 services
   `sudo microk8s.enable dashboard registry` (`dns` was already enabled for me)
   
   | Add on | Purpose |
   | --- | ----------- |
   | DNS | 	Deploys the coreDNS service. |
   | Dashboard | Deploys the `kubernetes-dashboard` service and several other services that support its functionality. It's a general-purpose, web-based UI for Kubernetes clusters. |
   | Registry | Deploys a private registry and several services that support its functionality. To store private containers, use this registry. |

#### Explore the Kubernetes cluster
Recall from earlier that a Kubernetes cluster is composed of control planes and worker nodes. Control plane is responsible for maintaining the desired state of the cluster, while the worker nodes actually run the applications and workloads.

MicroK8s provides a version of `kubectl` that you can use to interact with your new Kubernetes cluster. This copy of `kubectl` allows you to have a parallel installation of another system-wide `kubectl` instance without affecting its functionality.

1. Run the `snap alias` command to alias `microk8s.kubectl` to `kubectl`. This simplifies usage.  
   `sudo snap alias microk8s.kubectl kubectl`  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fdc075da-38d7-4b3e-ba9f-22eac0cf5dad">

2. Check the nodes running in your cluster  
   You know that MicroK8s is a single-node cluster installation, so you expect to see only one node. Keep in mind, though, that this node is both the control plane and a worker node in the cluster.

   `sudo kubectl get nodes`  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6af17180-5be2-423b-9b46-b00e6690d72c">  
   The ready state indicates that the control plane might schedule workloads on this node.
   
   You can get more information for the specific resource that's requested. For example, let's assume that you need to find the IP address of the node.

   `sudo kubectl get nodes -o wide`  
   <img width="850" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/657069fb-c280-4ed4-afd1-89072b37919b">

3. Explore the services running on your cluster  
   `sudo kubectl get services -o wide`

    <img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5de59576-5892-4e5a-b4da-32448cd84f35">     
    Notice that only one service is listed. You installed add-ons on the cluster earlier, and you'd expect to see these services as well. The reason for the single service listing is that Kubernetes uses a concept called namespaces to logically divide a cluster into multiple virtual clusters.

    To fetch all services in all namespaces, pass the `--all-namespaces` parameter:

    <img width="850" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/db756ded-70d4-4f41-9590-724b3101ad7f">

   Notice all the namespaces.

#### Install a web server on a cluster
You want to schedule (means assigning a pod to a node) a web server on the cluster to serve a website to your customers. Kubernetes scheduler is responsible for this assigning task.

Recall from earlier that you can use pod manifest files to describe your pods, replica sets, and deployments to define workloads.
Because you haven't covered these files in detail, you run kubectl to directly pass the information to the API server.

Even though the use of `kubectl` is handy, using manifest files is a best practice. Manifest files allow you to roll forward or roll back deployments with ease in your cluster. These files also help document the configuration of a cluster.

1. **Create deployment**  
   Specify the name of the deployment and the container imahe to create a single instance of the pod.  
   `sudo kubectl create deployment nginxdeploy --image=nginx`
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/465372c3-3d3a-4333-9541-9653f6ed0d3b">

2. **View deployments**  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/aae09952-8ea0-44e3-97a8-e26323549ee0">

3. **View Pods**  
   The deployment from earlier created a pod. View it.  
   `sudo kubectl get pods`  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/313d83fd-1872-4695-9774-76a5deb90be0">  
   Notice the name of the pod is generated using the deployment name I gave earlier.

#### Test the website installation
1. **Find the address of the pod**  
   `sudo kubectl get pods -o wide`  
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a8fe7f88-f81b-4a9a-ae8a-ef3e345ed74e">

   Notice that the command returns both the IP address of the pod and the node name on which the workload is scheduled.

2. **Access the website**  
   `wget 10.1.254.73`  
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/473b0a7e-4931-4363-b597-dc9f2675892a">

#### Scale the webserver deployment on a cluster
To scale the number of replicas in your deployment, run the `kubectl scale` command. You specify the number of replicas you need and the name of the deployment.

1. **Scale NGINX pods to 3**  
   `sudo kubectl scale --replicas=3 deployments/nginxdeploy`  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c2ca06bb-0597-4072-9b70-5abe18c4801d">

2. **Check the pods**  
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/36e1b878-4250-4a63-9541-3411a38ae583">

You'd need to apply several more configurations to the cluster to effectively expose your website as a public-facing website. Examples include installing a load balancer and mapping node IP addresses. This type of configuration forms part of advanced aspects that you'll explore in the future.

### When to use and not use K8s
#### Use K8s
You want to use Kubernetes when your company:  
1. Develops apps as microservices.
2. Develops apps as cloud-native applications.
3. Deploys microservices by using containers.
4. Updates containers at scale.
5. Requires centralized container networking and storage management.

#### Don't use K8s
For example, the effort in containerization and deployment of a monolithic app might be more than the benefits of running the app in Kubernetes. A monolithic architecture can't easily use features such as individual component scaling or updates.

### Uninstall MicroK8s
Before uninstall:  
<img width="100" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/48d515fe-2e7c-4e36-b324-8c39876d9ac0">

1. **Remove add ons from the cluster**  
   `sudo microk8s.disable dashboard dns registry`

2. **Remove MicroK8s from the VM**  
   `sudo snap remove microk8s`

3. **Exit the VM**  
   `exit`  
   <img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2add3d14-da0d-4dea-beba-fa0ce5a92312">

4. **Stop the VM**  
   `multipass stop microk8s-vm`

5. **Delete and purge the VM instance**  
   `multipass delete microk8s-vm`
   `multipass purge`

After uninstall:  
<img width="100" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5fe17dad-8cd3-45ec-ac17-980b00ac126d">

---

Nana's K8s course...
---

### Other `kubectl` commands
1. Get Logs from Pod  
   `kubectl logs [pd name]`  

2. Describe Pod  
   `kubectl describe pod [pod name]`

3. Debugging by getting into the pod and get the terminal (bash for eg.) from in there  
   `kubectl exec -it [pod name] -- bin/bash`
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/789efdfc-1d77-4bfe-8392-24b433c70c98">

3. Delete Pods  
   `kubectl delete deployment [name]`  
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b549874e-87a1-4fd6-ba1e-fc2625d0e5d8">

4. Using config files to do deployments  
   `kubectl apply -f [some-config-file.yaml]`
   The first `spec` is for deployment and the second `spec` is for pods.  `template` is the blueprint for the pods.  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/624db0b7-1660-49e2-9dd2-056f6ed9b962">

**Summary:**  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/847d015b-1085-459f-94d5-22b7f48c3559">  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/54533231-60fe-4f3f-98c9-504bb5ad2d71">

### K8s YAML config file
YAML is a human friendly data serialization standard for all programming languages.  
Its syntax uses strict indentation.  
Best practice is to store config file with your code (or have a separate repo for config files).

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/754f9518-250a-4802-8720-9cd1521f6c0b">

Each configuration file has 3 parts:  
1. Metadata
2. Specification
3. Status (This part is automatically generated and added by K8s)  
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/333a0dc3-e0a7-4879-b857-5b0c679679c5">

   K8s gets status data from `etcd` database.
   `etcd` holds the current status of any K8s component.

#### Blueprint for Pods (Template)
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2fd8c270-f25b-4be1-a802-0e565390599d">

#### Connecting Components (Labels & Selectors & Ports)
`metadata` part contains labels and `spec` part contains selectors.

1. Connecting Deployment to Pods  
   Way for deployment to know which Pods belong to it.
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/eaa3face-37d5-4149-8963-00b5b08b2549">

   Pods get the label through the template blueprint.
   Then we tell the deployment to connect or to match all the labels `app: nginx`.

2. Connecting Services to Deployments  
   Way for service to know which Pods belong to it.
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5ae2b2a1-f84b-48db-bf8a-25be3247c6c2">

   Example:
   Service has `spec:ports` configuration and deployment has `spec:template:spec:containers:ports` configuration:  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8fcb98bd-fc49-4acc-93b3-970148742ed8">

#### Ports in Service and Pod
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fbcaa9c7-61a1-498c-864f-0f71af2d10e6">

#### Example
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/21ec293e-4579-44d5-907e-acff811b436b">

#### View the status of the deployment generated by K8s from `etcd` and compare it to original
`kubectl get deployment nginx-deployment -o yaml > nginx-deployment-result.yaml`

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a7aea357-a4f9-4ab1-a81e-c470a5dc3060">

#### Delete the deployment and service
`kubectl delete -f nginx-deployment.yaml`  
`kubectl delete -f nginx-service.yaml`

### Elaborate [example](https://youtu.be/X48VuDVv0do?si=-WTbSOZ04U-VLaCJ&t=4576)
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c0420032-d53b-4a61-8fab-65de95579945">

#### Create MongoDb deployment
Create secret first before you run deployment. Secret is created in the section that immediately follows this one.

`mongodb-deployment.yaml`

````
# This file will get checked into app's Git Repo, so don't put secret value of settings like MONGO_INITDB_ROOT_USERNAME
kind: Deployment
metadata:
    name: mongodb-deployment
    # A service could use 'app:mongodb' selector to route network traffic to the pods created by this deployment. A label is used to identify the deployment itself.
    labels:
        app: mongodb
spec:
    replicas: 1
    selector:
        matchLabels:
            app: mongodb
    template:
        metadata:
            labels:
                app: mongodb
        spec:
            containers:
            # Config this image from info here: https://hub.docker.com/_/mongo
            - name: mongodb
              image: mongo
              
              ports:
              - containerPort: 27017
              
              env:
              - name: MONGO_INITDB_ROOT_USERNAME
                valueFrom:
                    secretKeyRef:
                        name: mongodb-secret
                        key: mongodb-root-username
              - name: MONGO_INITDB_ROOT_PASSWORD
                valueFrom:
                    secretKeyRef:
                        name: mongodb-secret
                        key: mongodb-root-password
````

Create Deployment:

`[~]$ kubectl apply -f mongodb-deployment.yaml`  

#### Create secret (this will live in K8s, not in repo)
Create secret file:

`mongodb-secret.yaml`

````
apiVersion: v1
kind: secret
metadata:
    name: mongodb-secret
# Basic key:value secret type. Other types include TLS
type: Opaque
data:
    # The value you put here should be base64 encoded
    # Get the base64 value by going to terminal and typing:
    # echo -n 'username' | base64
    # Similarly using 'password' for password
    mongodb-root-username: dXNlcm5hbWU=
    mongodb-root-password: cGFzc3dvcmQ=
````

Create Secret:

`[~]$ cd k8s-config/`  
`[~]$ ls`  
`mongodb-deployment.yaml      mongo.yaml`  
`[~]$ kubectl apply -f mongodb-secret.yaml`  
`[~]$ secret/mongodb-secret created`  

`k8s-config` is just a folder on your local computer.

You can view secrets with command:  
`kubectl get secret`

#### Create MongoDb Internal Service
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/79eee145-8f9f-4e20-b663-5313a7c25661">

Add this section to the `mongodb-deployment.yaml` file.

````
---
apiVersion: v1
kind: Service
metadata:
  name: mongodb-service
spec:
  selector:
    # To connect to Pod through label
    app: mongodb
  ports:
    - protocol: TCP
      # Service port
      port: 27017
      # This is the containerPort of deployment
      targetPort: 27017
````

Create Service:

`[~]$ kubectl apply -f mongodb-deployment.yaml`  
`deployment.apps/mongodb-deployment unchanged`  
`service/mongodb-service created`  

To check that the service is attached to the correct pod:  
`kubectl describe service mongodb-service`  

<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1ef6b94c-27cc-42ea-a4bd-69835d7011cc">  
That's the POD IP address and 27017 is the port where the application inside the Pod is listening.

Check that IP on the Pod by running:  
`kubectl get pod -o wide`  

#### View all components created so far
`kubectl get all | grep mongodb`

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b6c20141-e2ed-4712-80d1-05dc73912828">

#### Create External configuration to put Db url for MongoDb
`mongodb-configmap.yaml`

````
apiVersion: v1
kind: ConfigMap
metadata:
  name: mongodb-configmap
data:
  # Servername is just the name of the service
  database_url: mongodb-service
````

Create config map:  
`kubectl apply -f mongodb-configmap.yaml`

#### Create MongoExpress deployment
Use config from here: https://hub.docker.com/_/mongo-express

You need these info:  
1. Which Db it should connect to?  
   Environment var: `ME_CONFIG_MONGODB_SERVER`
2. Credentials to authenticate using.  
   Environment var: `ME_CONFIG_ADMINUSERNAME` and `ME_CONFIG_ADMINPASSWORD`

`mongoexpress-deployment.yaml`

````
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mongo-express
  labels:
    app: mongo-express
spec:
  replicas: 1
  selector:
    matchLabels:
      app: mongo-express
  template:
    metadata:
      labels:
        app: mongo-express
    spec:
      containers:
      - name: mongo-express
        image: mongo-express
        ports:
        - containerPort: 8081
        env:
        - name: ME_CONFIG_MONGODB_ADMINUSERNAME
          valueFrom:
            secretKeyRef:
              name: mongodb-secret
              key: mongo-root-username
        - name: ME_CONFIG_MONGODB_ADMINPASSWORD
          valueFrom: 
            secretKeyRef:
              name: mongodb-secret
              key: mongo-root-password
        - name: ME_CONFIG_MONGODB_SERVER
          valueFrom: 
            configMapKeyRef:
              name: mongodb-configmap
              key: database_url
````

Create deployment:  
`kubectl apply -f mongoexpress-deployment.yaml`

#### Create MongoExpress External Service
To access mongoexpress from the browser.

Add this section to the `mongoexpress-deployment.yaml` file.
````
---
apiVersion: v1
kind: Service
metadata:
  name: mongo-express-service
spec:
  selector:
    app: mongo-express
  # Add this guy that's different from internal service
  # Bad naming choice because internal service also works as a load balancer
  # This makes the service accept external requests by assigning the service an external IP address
  type: LoadBalancer  
  ports:
    - protocol: TCP
      port: 8081
      targetPort: 8081
      # Add this guy that's different from internal service
      # Port where the external IP address will be open. Must be between 30000 to 32767
      nodePort: 30000
````

Create external service:  
`kubectl apply -f mongoexpress-deployment.yaml`

Check it out:  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ac0b67c1-2c53-4300-b776-ca515fbf6815">

Internal Service or Cluster IP is DEFAULT.  
LoadBalancer is also assigned an External IP.

#### Give a URL to external service in minikube
`minikube service mongo-express-service`

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/884d849b-96a5-409f-87d4-37e48eb82ce8">

### Namespace
Way to organize resources.

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/dfdf89d7-9a93-49e4-9eff-0f1902c487fd">

You can put your object/ resource into some namespace in the .yaml configuration file.  
<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6b58171f-c4a1-4863-91f4-80f5b7812987">

**Why?**  
1. To group resources into different namespaces.
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/699c0798-624e-4374-8fe5-18af861fef02">

2. Use same cluster by different teams.  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6d2809d6-83b0-49cd-aceb-dc40f09f4e17">

3. Resource sharing: Staging and Development.  
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5da2c1b1-931e-4796-b972-570a517ef9a2">

4. Set access and resource limits.  
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/80a7b5cb-fbbb-4f24-9aa6-9575d4944bc9">

You **can access** service in another Namespace:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d427f54a-b0f1-4623-81ce-2ec6bcf910f6">

You **can't access** most resources from another namespace.  
For eg: Each namespace must define its own ConfigMap, Secret etc.

There are some components that can't live inside a namespace.  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0a224443-b673-45b4-829e-b49a7749c7de">

### Ingress
To make an app accessible through the browser, you can use external service or ingress.

External Service looks like below. It uses http protocol and IP address:port of the node which is ok for testing but not good for final product.  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f43a4b47-8075-4ef3-bb91-269b7239b503">

Ingress makes it better:  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/08c8836d-d372-4c37-888a-80a9cb875635">

One thing to note is that the `http` attribute under `rules` doesn't correspond to http protocol in the browser.  
This just means that the incoming request gets forwarded to internal service.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a78853c4-b1ee-4082-99b1-6d9e484fd783">

#### Ingress and internal service configuration
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/88972574-7f1d-426f-8caa-b4df5b6065d8">

In the `host: myapp.com`, `myapp.com` must be a valid domain address.
You should map domain name to Node's IP address, which is the entrypoint. 

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f23b89da-c00e-451f-9ff5-686cc5c47764">

#### How to configure ingress in your cluster
You need an implementation for Ingress which is called Ingress Controller.

Install an ingress controller which is basically a pod or a set of pods which run on your node in your K8s cluster.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6edf9cb0-bbf1-4539-aee7-383903e7a45b">

#### Ingress Controller
1. Evaluates all the rules defined in the cluster.  
   <img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b6e2ba98-c58b-4999-b773-2e87c3ac8705">

2. Manages redirections.
3. Entrypoint to cluster.
4. Many third-party implementations.

If you're running your K8s cluster on some cloud environment, you'll already have a cloud load balancer.
External requests will first hit the load balancer and that will redirect the request to Ingress Controller. 

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e8228d68-cadd-4970-b66c-510223223ca8">

But if you're deploying your K8s cluster on a bare-metal, you'll have to configure some kind of entrypoint yourself. That entrypoint can be either inside of your cluster or outside as a separate server.  
For eg: An external Proxy Server shown below.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8499ad31-76a4-41e1-ae5d-0924cef3b327">

#### Install Ingress Controller in Minikube
`minikube addons enable ingress`

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d27794bd-aedc-4e65-906e-b914881c8c71">

With 1 simple command, I can have Nginx ingress controller pod setup.

#### Create Ingress rule for K8s dashboard
`kubectl get ns` just lists all the namespaces in your current K8s context.  
<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/74d046fb-ed95-4daf-8198-7cea9f592ae3">

`kubectl get all -n kubernetes-dashboard` shows all the components in kubernetes-dashboard.  
It already has internal service and pod for K8s dashboard (you can see that if you run the command above).

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0d80b601-6a6c-4c03-98f4-17775b2e183f">

Now we can configure an ingress rule for dashboard so it's accessible from our browser using some domain name.

`dashboard-ingress.yaml`

````
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: dashboard-ingress
  # Same as service and pod!
  namespace: kubernetes-dashboard
spec:
  rules:
  - host: dashboard.com
    http:
      paths:
      - backend:
          service:
            name: kubernetes-dashboard
            port: 
              number: 80
````

Above is the ingress config for forwarding every request that is directed to `dashboard.com` to internal `kubernetes-dashboard` service.
`kubernetes-dashboard` service is internal service as seen by it being of `ClusterIP` type.

Create ingress rule:

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/784a680b-ffaa-466a-9c6a-6dc0d212a30e">

#### Resolve IP address to host name `dashboard.com`
Go to your hosts file and put that mapping.

`[~]$ sudo vim /etc/hosts`

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/93259521-3b0c-4526-b34a-ad7fe9a1d555">

Type `Esc` and `:wq` to save and exit.

Now go to your browser and type `dashboard.com` and you'll reach the K8s dashboard.

#### Ingress default backend
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e594e2e9-1228-40c8-a946-9c9ab7693a8c">

A good usage for this is to provide user with some custom error messages when a page isn't found.

For this, all you have to do is create an internal service with the name `default-http-backend` and create a pod or application that sends the custom error response.  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/85c7e0b0-aaf9-4f8d-9cba-c497f64599c5">

#### Multiple paths for same host
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6d78b75f-bb8a-44c0-b4e8-bc6111e88214">

#### Multiple sub-domains or domains
You can also have multiple hosts with 1 path. Each host represents a subdomain.

<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e0e61b6e-ab95-4335-8215-64a18fb42727">

#### Configuring TLS certificate - https
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bd5a0a9f-7165-4030-96c6-a4217aee484c">

Notes:  
1. Data keys need to be "tls.crt" and "tls.key".
2. Values are file contents NOT file paths/ locations.
3. Secret component must be in the same namespace as the Ingress component. `default` in the example.

### Helm
Package manager for Kubernetes.  
To package YAML files and distribute them in public and private repositories.

For eg: You have deployed your app in a K8s cluster. Now you want to deploy/ add elastic stack for logging.  
If I wanted to do this myself, I'd have to take care of the following components:
1. Stateful set: For stateful apps like Dbs
2. ConfigMap: For external config
3. K8s user with permissions
4. Secret: For secret data
5. Services

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/9e221f9d-e32f-43b1-94c6-7aaae9f8a1af">

If I had to do this manually, it'd be tedious.  
But since something like Elasticsearch is a standard deployment, someone already created the YAML files required for those deployments, packaged them up and made them available in repositories so other people could use them.

That bundle of YAML files is called **HELM chart**.

#### Helm Charts
1. Bundle of YAML files
2. Create your own Helm Charts with Helm
3. Push them to Helm Repository
4. Download and use existing ones

Standard apps with complex setup like: Database apps: MongoDb, MySQL, Elasticsearch, Monitoring apps: Prometheus etc. all have charts already available.

#### Helm is a templating engine
If you have an app that has a bunch of microservices and deployment and service configuration of each of those microservices look almost the same. For eg: The only difference is something simple like version and image name.  
In this scenario, you can just define a base/ common blueprint and use placeholders to populate dynamic values.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/3931aca8-749a-4200-a0c3-577b69455f32">

The values come from `values.yaml` file.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f248feaf-2c03-41c2-a705-797405a1c787">

#### Values injection into template files
`helm install --values=my-values.yaml <chartname>`

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/039605b5-2e17-4e06-850f-17445683f955">

#### Helm helps deploying same app across different environments
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e0d3f594-f21a-4b81-a733-b5318a3c695d">

#### Helm chart structure
<img width="200" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/124b26be-9dae-4b47-9e93-e162c474195f">

1. mychart folder: Name of the chart.
2. Chart.yaml: Meta info about the chart. For eg: name, version, dependencies etc.
3. values.yaml: Values for the template files.
4. charts folder: Chart dependencies. For eg: If this chart depends on other charts.
5. templates folder: Actual template files.

Install chart using the command:  
`helm install <chartname>`

#### Helm version 3 got rid of Tiller for good reasons

### Kubernetes Volumes
Persist data in K8s using following ways:  
1. Persistent Volume
2. Persistent Volume Claim
3. Storage Class

Let's say you have an app with a Db.  
<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/69fa3fb5-0cdc-42f3-9fbd-4a4637765c3b">

If you make changes to the db and restart the `mysql` pod, the data gets lost because K8s doesn't provide data persistence out of the box.

**Storage Requirements:**  
1. We need a storage that doesn't depend on the pod lifecycle.  
   <img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/7584ed59-ff6d-475b-b705-cffb41323a3d">
2. Storage must be available on all nodes.  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/94987239-4ba4-46e5-a2c9-56c3f6db9c4b">
3. Storage needs to survive even if cluster crashes.

#### Persistent Volume
1. Cluster resource.
2. Created via YAML file.
3. Needs actual physical storage.  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e65c8cf7-7d25-47e0-a3c8-e9307869243f">
4. Example that uses Google cloud:
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/af252f0c-4df7-45d3-90e3-5aec28e68ae4">
5. They are NOT namespaced.
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ebc1eea6-1336-4c6d-b8ef-bb85f45f2356">

#### Persistent Volume Claim
Application has to claim the Persistent volume.

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1a383fc1-829c-4ea6-acc8-8ff59232e688">

In other words, PVC specifies claims about a volume with storage size, access mode etc. and whichever persistent volume matches those claims will be used for the application.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a2bd62e8-f5d4-40ae-aa48-1673ae95f8b3">

Note: PVClaims must exist in the same namespace as the pod.

You use that claim in the Pods configuration like so:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fee3ae01-a2e6-48f0-a149-ae3f9823df03">

**Mounting volume:**  
<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4d860872-729b-41e2-ad93-4c4ecde0411c">

#### Different volume types in a Pod
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/df181cc9-b598-48f9-b87b-8700650c4b50">

#### Storage Class
Storage class provisions persistent volumes dynamically when PersistentVolumeClaim claims it.  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a356b34f-e2b1-42ad-8cf0-58124dab2754">

Using Storage class:  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/04677f2e-2834-416f-86cc-c99e44fca23f">

Flow:  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/87506ae1-55ae-40d5-b8c3-1b0f35034891">

#### Volume Types: ConfigMap and Secret
1. They both are local volumes.
2. They are not created via PV or PVC.
3. They are managed by K8s.

### StatefulSet
Stateful vs Stateless apps

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c58b7dc3-befe-436c-8bbf-0796aa3567d4">

#### Deployment vs StatefulSet
**Deployment:**

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e43dcd7d-baf8-4d56-9584-d89475c879ac">

When you delete one, one of those pods can be deleted randomly. 

**StatefulSet:**

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/71a6477b-143f-4d80-aa04-6fcc427b3905">

#### Pod Identity

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1e3c1181-a506-41c4-8746-6fecee9048b4">

Let's say `ID-2` pod goes down and created again. Even when it's created newly this time, it'll have the same Id of `ID-2`.

#### Scaling Db apps
Only let one instance to write data to prevent data inconsistencies.

Let's say you add `mysql-3` instance.  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/284d1af5-84ba-48cd-bae7-9328cf9b15a6">

The workers continuously synchronize data.  
PV means Persistent volume.

#### Pod state 
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/036fb5c8-bcb1-4fa2-9f66-5a38383a0b93">

Pod state is stored in its storage. When a pod dies and it gets replaced, the persistent pod identifiers make sure that the storage volume gets reattached to the replacement pod.

#### Pod Identity
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/344b5add-a958-45f7-9efd-7dd07905c50b">

For eg: StatefulSet with 3 replica:
1. mysql-0 : Master
2. mysql-1 : Worker
3. mysql-2 : Worker

Next pod is only created if previois one is up and running.  
Deletion in reverse order, starting from the last one.

#### Pod Endpoints
Each pod in a stateful set gets it own DNS endpoint from a service.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8cb0d78d-8e29-483d-b471-ebf6d66b1199">

2 characteristics:  
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a4bbcbc5-21cb-4494-aa64-2aa766578a99">

#### It's complex so just use cloud db services.
Stateful apps are not perfect for containerized apps, stateless are.

### Kubernetes Services
Pods in K8s are destroyed frequently, so it's not practical to reach them using their ip addresses.
Services address this problem by providing a stable IP address even when the pod dies.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f677caad-fd51-47eb-b1cd-943194ddd6c2">

**Different Services:**
1. ClusterIP Services
2. Headless Services
3. NodePort Services
4. LoadBalancer Services

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2dfe7d6e-90a5-4407-975c-5b2cc5afc79b">

#### ClusterIP Services
This is a default type, so when you create a service and don't specify the type, this is what you'll get.

<img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c87f3b55-c180-4008-a9a8-a79ab1b299a9">

For eg:  
Let's say we have deployed our microservice app in our cluster. Let's say this pod contains the app container and a sidecar container that collects logs from the app and sends it to some log store. The deployment yaml file looks like this:  
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ee490118-8260-400e-b21a-7a20c0e78557">

**Pod gets an IP address from a range that is assigned to a Node.**

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d48f38c7-1f29-4667-9adc-60501d235eb7">

Let's say you access the app through the browser:

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cbf516f7-4038-4d7f-b443-63db0cb4f932">

The ingress rule and app service yaml contents look like this:

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8fa1242c-a1f5-485a-b81c-baaf5101cf80">  
As you can see, the ingress forwards to port 3200 where the service is, and the service forwards to port 3000 in the container where the app is running.

The Service port is arbitrary while targetPort is not. targetPort has to match the port where container inside the pod is listening at.

**Which pods to forward request to?**  
By using selectors which are labels of pods.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f449dc1d-2fbb-4aba-b1b9-c0d9feb2186c">

**Which port to forward request to?**  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/3cb110d9-424d-4988-a028-e3cbc6cf920e">

When we create the service, it will find all the pods that match the selector (1), so these pods will become **endpoints** of the service.
When service gets a request, it will pick one of those pod replicas randomly beause it's a load balancer and it will send the request it received to that specific pod on a port defined by `targetPort` attribute (2).

**Service Endpoints:**  
When we create a service, K8s creates an endpoints object that has the same name as the service and it will use the endpoints object to keep track of which pods are members/ endpoints of the service.

<img width="380" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6f335791-f1c7-4877-a1e6-9d7d52bb7136">

**Multi-Port Services:**  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f04cb733-52ad-43d1-a835-499867807101">

When you have multiple ports defined in a service, you have to name them.

<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/32f013d4-2240-461d-a0c5-829478b9e1c0">

#### Headless Services
Useful in scenario like:
1. Client wants to communicate with 1 specific Pod directly.
2. Pods wants to talk directly with specific Pod.
3. Pod can't be randomly selected.

**Use case:** Stateful applications, like databases where Pod replicas aren't identical.

To create a headless service, set clusterIP to none.  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/48b67607-a0fb-4652-a069-7150e9b88233">

We have these 2 services alongside each other. ClusterIP one will handle load balanced requests, Headless one will handle data synchronization requests.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ae95e98b-5227-4a7d-a7af-4a2e5f7925fe">

#### NodePort Services
Recall that CLusterIP is only accessible within the cluster so no external traffic can directly address the cluster IP service.
The node port service however makes the external traffic accessible on static or fixed port on each worker node.

So in this case, instead of a browser request coming through the Ingress, it can directly come to the worker node at the port that the service specification defines.
This way external traffic has access to fixed port on each Worker Node.

This is how NodePort service is created.
Whenever we create a node port service, a cluster ip service to which the node port service will route is automatically created.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ec3c57fc-2628-4cb9-82b7-5a69fb8f7f0a">

The nodePort value has a predefined range of 30000 to 32767.
Here you can reach the app at `172.90.1.2:30008`.

**View the service:**  
The node port has the cluster IP address and for that IP address, it also has the port open where the service is accessible at.  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/941eb8bd-0a3b-44cf-b3f4-7cfe7ed4aa47">

**Service spans all the worker nodes:**  
Service is able to handle a request coming on any of the worker nodes and then forward it to one of the pod replicas. Like `172.90.1.1:30008` or `172.90.1.2:30008` or `172.90.1.3:30008`. This type of service exposure is not secure.  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4ea4d459-2053-4784-8eb6-713b9936d3ef">

Don't use it for external connection. Use it only for testing.

Configure Ingress or LoadBalancer for production environments.

#### LoadBalancer Services
A service becomes accessible externally through a cloud provider's load balancer service. 

LoadBalancer service is an extension of NodePort service.  
NodePort service is an extension of ClusterIP service.  
So whenever we create LoadBalancer service, NodePort and ClusterIP service are created automatically.

The YAML file looks like this:  
<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0e41ac26-8297-4c25-8881-59b261b37261">

It works like this:  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c717755a-24ad-4a2d-98bb-39b9391906d1">

NodePort is the port that open on the worker node but it's not directly accessible externally but only through the load balancer. 
So the entry point becomes the load balancer first and it can then direct the traffic to node port on the worker node and the cluster IP, the internal service.

The load balancer service looks like this:  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4706aeb2-8daf-4364-a820-ede12058f9c9">

Configure Ingress or LoadBalancer for production environments.

----

Malai tyo bela samma thaha thiyena ki bhauju le mero relationship ramro banauna help garnu huncha ki nai bhanera teivara maile bhauju lai fully trust gareko thye. Tara jaba bhauju le ni timlai uchaleko dekhe ani tespachi maile bhauju ko sabai kura sunna hudaina jasto lagyo.

Malai ta timi sita matra haina sabai sita fully honest huna man lagcha, biswas garne nagarne timro icchya.

Malai bhauju ko yestai kura le ho sabai kura sunna hudaina bhanne lageko.

Ho kinaki timi atti nai closed minded chau ni ta. "Maile suneko kura matra thik ho, mero husband le bhaneko kura sab galat ho" bhanne mentality cha ni ta timro.

Malai bhane myth, misconception, suneko kura etc. haru ma fact khojna man lagcha, research garna man lagcha, bujhna man lagcha, tara timlai chai afno husband bahek aru le bhaneko sab thik ho, afule kei pani dimakh launa hudaina bhanne khalko mentality cha ni ta.

Khai yo audio ma ke bhanna khojeko maile ramro sita bujhina. Ma ta bhanchu je kura ma ni afno dimakh lagauna parcha, kei kura logical nalage internet ma search garnu parcha, ani conclusion nikalne ho afaile.

Bhauju le aba regular salt bhanda pink salt ramro bhanera kun uncle sita ke reason le bhannu bhayo malai thaha chaina, tyo bela timle sodheko bhaye hunthyo ali ramro sita bhauju lai ni sidhai. Mero research anusar chai Pink salt ma hune mineral ko amount yeti thorai huncha ki telle health ma kei affect nai gardina, ra tesma iodine nahune karan le tyo use garnu healthy hudaina bhanne ho. Maile tyo kura kunai doctor le sunayera bhaneko haina, afai research garera thaha bhayeko kura ho.
Ra Bhauju Nutritionist pani hoina, teivara Bhauju ko kura or kunai pani doctor ko nutrition related kura blindly sunnu ramro hoina.
Kasaiko kura ma ni blind faith rakhnu bhaneko murkhata ho. 

Yo kura ma sorry, ki timle bhana ki Aama le bhannu huncha. Yesma Mama le bhanne kei kura chaina.
Yeha ris uthne kura ke ma cha bhanda timle or Aama le kailei pani galti own nagareko bhayera ho.
Malai Mama le CLEARLY bhannu bhako thyo "Aama le Washing machine pathaidincham" bhaneko ho bhanera tara Aama le testo bhaneko chaina bhannu bhayo.
Maile aba kasko kura pattyaune? Malai yesari ghumayera kura gareko man pardaina.

Ye baba, timle kura nabujheko ho ki nabujhe jasto gareko ho?
Maile timro bhagwan ko kasam Sali harlai "ke testo taar wala paat chij dinu" bhanera Airpods kineko ho. Dekhaune bhanne mero secondary kura ho. Main kura ramro saman use garun bhanne ho, secondary kura ramro diyechan bhanera sabai le sochun bhanera ho.
Yeti jabo kura ma kina niu khojchau ho? Yellai positively lina sakdainau? "La tani mero husband ko family le ke-ke diyenan bhanne sunecha, teivara ramro chij kinera kura katne haru lai dekhauna khojeko raicha" bhanera liu na. Kina yo kura ma masita fight garnu paryo ra?

Ho straight kura garda of course help huncha tara hamro problem ke cha bhanda timi honest huna sakdainau, timi bhaneko manna sakdainau, timi maile bhaneko kei kura sunna pani ready chainau ani kasari ramro huncha relationship?
Maile bhaneko mana, suna, ani honest bhau ta, I promise everything will get better.
Tyo timlai chitta bujhdaina bhane ma pani timi jastai kura lai 100 fera batarera, jhut bolera basum ta? I don't know how that would help us.

Malai thaha thiyena manche haru timi jasto dimkah sadkeko hunchan bhanne teivara tyo kura ma maile ramro sita dhyan nadinu mero galti ho.
Spiritual bhanne nonsense liyera je pani "mero bhagwan - mero bhagwan" bhanera basne, afule chai kei mihinet nagarne, chahine kura ma alchi matra garna khojne, khali japera basne, afno dimakh kei pani use nagarne, yesto closed minded chau bhanne kura maile dhyan dinu parne thyo, maile diyina, teha mero galti bhayo

Tyo bela fact bhanna sajilo thyo, ma koi thiyina tyo bela. Tara aba husband-wife bhayesi fact bhanna bhanda ghumauna thik lagcha timlai.

Maile tyo bhaneko kina bhanda I was frustrated ki timi mero kura kailei sunna, bujhna ready nabhayeko dekhera ho.
Yettikai niu khojeu tyo bela: "Thulmommy lai bhanera nalyauna bhanchu" bhanera. Tyo bela maile ke clear gareko bhanda: yedi timle yo kura ma malai jabaf dina ko lagi Thulmommy lai bhaneu bhane you don't respect me one bit, teivara malai lagyo ki mero wife le mero kura kei pani sundina, respect pani gardina bhane yo relationship ko ke kaam bhanera, we'll be done bhaneko.
Aile farkera herda, that was a little too much. Maile teso bhanna hunthena ris ko jhok ma, I'm sorry.

Yeha problem ke bhairako cha bhanda you don't respect me or this relationship more than your preconception and your ego.
Timle maile bhaneko kura lai kailei pani seriously liyeko chainau, maile je bhane pani baal hanchau or negatively linchau or 100 fera ghumayera tarchau. Ani malai chitta bujhdaina ani maile ego dekhaye bhanne huncha.
Maile timlai aile samma nachahine kura ke ma pressure gareko chu?
Maile bhaneko timlai: timle yo Spirituality, ISKCON ma je je sikeu yo kura haru ma dherai jhut, propaganda cha, yo kura haru follow garna blind faith chahincha, blind faith bhaneko closed mindedness ho, yelle manche lai ekohoro banaucha, illogical banaucha, irrational banaucha teivara yo kura choda, baru ramro sita pada, ramro sita khau, afno body ko take care gara, ghar ma mommy le bhaneko suna, mana bhaneko ho.
Yesma naramro chai maile ke bhane? Ho maile risayera bhane hola tyo mero mistake ho (I'm sorry), tara timle tei kura sunne ho, manne ho bhane yeha ke issue cha? Yeha issue ayeko timle malai na trust garchau, na maile bhaneko sunchau, ani maile je bhaneni ego dekhaye jasto huncha.

ISKCON ka manche le bhaneko kura ma chai blind faith rakhna parcha bhanne timi, tara mero kura ma chai reasonable faith pani narakhne, ani kasari huncha? Ani dosh jati chai sabai mero?

Yo relationship chalna malai timro faith chaiyeko cha, dina sakchau? Dherai haina 3 months maile bhaneko kura sabbai suna, mero kura lai seriously liu, malai positively liu, honest bhau, ghar ko kura duniya sita nagara, straight kura gara, maile bhaneko mana. 3 months tetti gardeu, afai positive change aucha. Aile timle malai afno manche jasto ni gardainau, malai trust garne bhanne kura ta tadha ko kura bhayo.


Yeso afai bichar gara, yo sansar bhagwan le design gareko jasto nai dekhidaina.

Yeti dherai evil cha sansar ma, yeti dherai pain and suffering cha. Cancer jasto rog lagera manche marchan, din ma sana sana bachcha hajaurau-hajar morchan tadpi-tadpi. "Existence is pain" bata mukti dina mareko hola ni bhanera bhanchau hola, tara bhagwan jasto maya ko khani le kosailai pida diyi-diyi exist garayera marnu bhanda ta exist nai nabanauna parne haina ra? For eg: Kunai bakhra (goat) lai janmayera, manche le katera khanu bhanda ta tyo bakhra najanmeko bhaye pida hunthena ni. Ani purba juni ko paap ko faal bhanera bhanlau, bhagwan jasto daya ko khani le purba juni ma paap garyo bhane arko juni ma tadi-tadpi marnu parcha bhanne thaha huda hudai kina afno sristi lai paap garne khalko banaunu bhayo hola ta? Present past future dekhne bhagwan le jani jani afno sristi lai paap garayera ako juni ma sajaya diyeko jasto bhayena ra logically sochda?

Ani Spirituality bhaneko state of mind ho, hamro mind yeti powerful cha ki kunai kura dherai sochyo bhane tesko illusion nai banaidincha.
Malai asti yo "Israel-Gaza" war ma bachcha haru lai bomb haneko dekhera yeti naramro lageko thyo ani maile yeti soche ki ma powerful bhako bhaye tyo war ma gayera ladthye, tyo bachcha haru lai bachauthye bhanera ani tyo sochda sochda maile room ma tyo war bhako thau ko tank gadi ko awaj suneko. Tei bhaye jastari suneko. Kassam. 
Teivayera yesta kura haru bhaneko sabai mind ko khel ho. Yesto kura lai dherai seriously liyo bhane mind nai weak bhaidincha.

Aba timlai universe ko origin, ani humans ko origin ma ekdam interest cha bhane Cosmology ra Theoretical Physics ko books haru kindimla, tyo pada ani khowledge pauchau. "Sabai bhagwan le banako" bhanne soch chai ekdam alchi soch ho, testo soch le kailei pani truth bhetinna, progress hudaina. 
Ra maile pade anusar, bujhe anusar, dekhe anusar yo sansar bhagawan le banayeko jasto chai dekhidaina kinaki bhagwan le banayeko sansar yo bhanda dherai peaceful, beautiful hunthyo ra bhagwan le afno sristi lai at least afno roop dekhaunu hunthyo. Tara aile ko bhagwan lai lukamari kheldai fursad chaina, paapi haru le bachcha haru lai bomb haneka chan ani tiniharu nai khusi chan, tiniharu nai dhani chan, tiniharukai family ramro cha.








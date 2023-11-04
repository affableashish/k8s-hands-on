# k8s-hands-on
Learning Kubernetes from [Microsoft Learn](https://learn.microsoft.com/en-us/training/paths/intro-to-kubernetes-on-azure/) and [freecodecamp](https://youtu.be/kTp5xUtcalw?si=OQgai8LBz8fttKoo) course.

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

![image](https://github.com/affableashish/k8s-hands-on/assets/30603497/e0de73ca-d817-4c80-b375-b0dfb5c7d47d)

[Reference](https://learn.microsoft.com/en-us/azure/architecture/patterns/strangler-fig)

## Cloud Native
Cloud native technologies empower organizations to build and run scalable applications in the cloud.
1. Uses containers, service meshes, microservices, immutable infrastructure and declarative APIs.
2. Enable loosely coupled systems that are resilient, manageable and observable. Combined with automation, they allow to make changes frequently.
3. Uses ecosystem of open source, vendor neutral projects.

View the Cloud Native Landscape here: https://landscape.cncf.io/

### Cloud Native Concepts - Speed and Agility

### Cloud Native Concepts - Application Architecture
![image](https://github.com/affableashish/k8s-hands-on/assets/30603497/dc5b9041-9838-4010-893a-4125e32c7892)

### Cloud Native Concepts - Mentality : Pets vs Cattle
Infrastructure becomes like cattle.

1. Infrastructure becomes immutable and disposable.
2. Provisioned in minutes and destroyed on demand.
3. Never updated or repaired, but re-provisioned.
   
### Cloud Native Concepts - Greenfield vs Brownfield
Cloud native projects are a lot easier with New projects but also possible with Legacy projects.

### Cloud Native Concepts - Cloud Native Trail Map
Breaks the journey into smaller measurable objectives.

Diagram here: https://raw.githubusercontent.com/cncf/trailmap/master/CNCF_TrailMap_latest.png
Taken from [here](https://github.com/cncf/trailmap).

## Containers
Finish the [Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/) training. It's pretty great.

Suppose you work for an online clothing retailer that's planning the development of several internal apps. Your team develops and tests all applications on-premises and then deploys them to Azure for pre-production testing and final production hosting. You're looking for maximum compatibility in each environment with little or no configuration changes. Using Docker as a containerization solution seems an ideal choice.

A **container** is a loosely isolated environment that allows us to build and run software packages. These software packages include the code and all dependencies to run applications quickly and reliably on any computing environment. We call these packages container **images**. **The container is the in-memory instance of an image.**

The container image becomes the unit we use to distribute our applications.

![image](https://github.com/affableashish/k8s-hands-on/assets/30603497/42ed1fcf-becf-411f-bc88-ddf17f8e0f54)

Docker container wraps an application's software into an invisible box with everything the application needs to run. That includes the operating system, application code, runtime, system tools, system libraries and so on. Docker containers are built off of Docker images. Since images are readonly, Docker adds a read-write file system over the read-only file system of the image to create a container.

### Container Image
A container image is a portable package that contains software. It's this image that, when run, becomes our container. **The container is the in-memory instance of an image.**

A container image is immutable. Once you've built an image, you can't change it. The only way to change an image is to create a new image. This feature is our guarantee that the image we use in production is the same image used in development and QA.

[Free Code Camp reference](https://www.freecodecamp.org/news/a-beginner-friendly-introduction-to-containers-vms-and-docker-79a9e3e119b/).

[Microsoft Learn reference](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/).

### VMs vs Containers
<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8731f9bc-291e-425d-b328-8d78103a07d7">
<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/700ae582-2f4a-431e-86b7-a7bbb88340e7">

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

The repository name must be of the form *<login_server>/<image_name>:<tag/>.

For eg: This is how repositories looks like:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/29cbde5c-6ad8-4ef6-8961-4f571f9414b7">

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/13325509-40fd-4b43-9b9f-f13e65b0f382">

Also take a look at [Microsoft Artifact Registry](https://mcr.microsoft.com/en-us/product/dotnet/samples/tags):

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1d5335a5-a36d-4ded-9802-7d2dba8e2eaf">

### How Image is created
Go to this site:
https://hub.docker.com/_/microsoft-dotnet-samples/

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bd9dfe43-2529-4cc3-a5d6-6eefaddcc3de">

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

## Docker commands
Follow this nice [exercise](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/3-exercise-deploy-docker-image-locally). 

1. Pull an image
   
   For eg: `docker pull mcr.microsoft.com/dotnet/samples:aspnetapp`

   When we pull an image, Docker stores it locally and makes it available for running it as containers.

2. View images
   
   For eg: `docker image list`
   | REPOSITORY | TAG | IMAGE ID | CREATED | SIZE |
   | --- | --- | --- | --- | --- |
   | mcr.microsoft.com/dotnet/samples | aspnetapp | 6e2737d83726 | 6 days ago | 263MB |

   Notice the repository name.

3. Run a docker container
   
   For eg:
   ````
   // Remember the port mapping format as H:C (Host:Container). Host first!
   docker run -p 8080:80 -d mcr.microsoft.com/dotnet/samples:aspnetapp
   ````
   The command maps port 80 in the container to port 8080 on your computer. So if you visit the page `http://localhost:8080`, you can see the running web app.
   
4. View active containers with the `docker ps` command.
   `ps` means "process status". It's a shortcut for `docker container ls`.
   Use `a` flag if you want to view stopped containers as well.
   
   Eg:
   | CONTAINER ID | IMAGE | COMMAND | CREATED | STATUS | PORTS | NAMES |
   | --- | --- | --- | --- | --- | --- | --- |
   | 57b9587583e3 | mcr.microsoft.com/dotnet/core/samples:aspnetapp | "dotnet aspnetapp.dll" | 42 seconds ago | Up 41 seconds | 0.0.0.0:8080->80/tcp | elegant_ramanujan |
   | d27071f3ca27 | mcr.microsoft.com/dotnet/core/samples:aspnetapp | "dotnet aspnetapp.dll" | 5 minutes ago | Up 5 minutes | 0.0.0.0:8081->80/tcp | youthful_heisenberg |
   
5. Stop a container
   
   For eg: `docker stop elegant_ramanujan`
   
6. Restart a stopped container
   
   For eg: `docker start elegant_ramanujan`
   
7. Remove a container
   
   Typically once a container is stopped, it should also be removed. Removing a container cleans up any resources it leaves behind. Once you remove a container, any changes made within its image filesystem are permanently lost.
   
   For eg: `docker rm elegant_ramanujan`

   You can't remove a container that's running, but you can force a container to be stopped and removed with the `-f` flag. Only use this iff the app inside the container doesn't need to perform a graceful shutdown.
   
   For eg: `docker container rm -f elegant_ramanujan`

8. Remove docker images
   
   For eg: `docker image rm mcr.microsoft.com/dotnet/core/samples:aspnetapp`.
   Containers running the image must be terminated before the image can be removed.

### Docker Commands cheat sheet
https://docs.docker.com/get-started/docker_cheatsheet.pdf

https://cheat-sheets.nicwortel.nl/docker-cheat-sheet.pdf

## Create custom image with a Dockerfile
To create a Docker image containing your application, you'll typically begin by identifying a **base image** to which you add files and configuration information.

A Dockerfile contains the steps for building a custom Docker image. Follow along this [guide at Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/5-exercise-create-custom-docker-image).

### Step 1: Clone this sample MSLearn repo to your projects folder:
`Ashishs-MacBook-Pro:RiderProjects ashishkhanal$ git clone https://github.com/MicrosoftDocs/mslearn-hotel-reservation-system.git`

### Step 2: Go into src folder
`Ashishs-MacBook-Pro:RiderProjects ashishkhanal$ cd mslearn-hotel-reservation-system/src`

### Step 3: Create a Dockerfile
`Ashishs-MacBook-Pro:src ashishkhanal$ touch Dockerfile`

### Step 4: Open Dockerfile in Vim
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

### Step 5: Save and exit
Hit `Esc` key and type `:` then type `wq` to write and exit the editor.

### Step 6: Build the image
This command builds the image and stores it locally.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker build -t reservationsystem:v1 .
````
`-t` flag specifies the name of the image to be created.
`.` provides the build context for the source files for the COPY command: the set of files on the host computer needed during the build process. So the first `.` in the COPY is `Ashishs-MacBook-Pro:src ashishkhanal$` which contains following files:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8b76c1fe-1071-4542-a076-304de49103a6">

### Step 7: Run the image
Give the container a name as `reservations`.
````
Ashishs-MacBook-Pro:src ashishkhanal$ docker run -d -p 8080:80 reservationsystem:v1 --name reservations
````
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f8695714-7ccf-46b4-b28b-219313498d1c">

After you're done, remove it: `docker rm reservations`

## Deploy Docker Image to Azure Container Instance
Follow this [exercise](https://learn.microsoft.com/en-us/training/modules/intro-to-containers/7-exercise-deploy-docker-image-to-container-instance).

### Create Azure Container Registry
1. Create a resource group
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cd2add06-7826-414f-aff5-9f9b951fb9ab">

2. Create container registry
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6031ce23-3884-4d53-b05a-23315a788ca6">

3. In the resource menu, under Settings, select Access keys. The Access keys pane for your container registry appears.
4. Enable the Admin user access switch.
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8cdcd70e-f32a-407c-a81d-d796a5f354c5">

5. Make a note of the Registry name, Login server, Username, and password.

### Tag an Image
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

### Push an Image
1. Sign into Azure Container Registry using `docker login <login-server>`
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/58d34abc-3fc7-44bb-9af9-f05a9effc59b">

2. Upload an image using `docker push <registry-name>.azurecr.io/reservationsystem:latest`
   ````
   Ashishs-MacBook-Pro:src ashishkhanal$ docker push myregistry5.azurecr.io/reservationsystem:latest
   ````

Login Issues if you don't use admin user:
https://stackoverflow.com/q/65316558/8644294

### Verify an Image
In the resource menu, under Services, select Repositories. The Repositories pane for your container registry appears.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/30f8b83b-352c-4d06-86c9-37147d4efd66">

### Run an Image
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

#### Controller manager
Controller manager launches and monitors controllers configured for a cluster through the API server. "through the API server" means that the controller manager uses the API server to interact with the K8s objects that the controllers manage.

#### Cloud controller manager





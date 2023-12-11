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

### Monolith to Microservices migration
Incrementally migrate a legacy system by gradually replacing specific pieces of functionality with new applications and services. As features from the legacy system are replaced, the new system eventually replaces all of the old system's features, strangling the old system and allowing you to decommission it.

<img width="800" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/e0de73ca-d817-4c80-b375-b0dfb5c7d47d">

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
   <img width="450" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/dc5b9041-9838-4010-893a-4125e32c7892">
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

<img width="450" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/42ed1fcf-becf-411f-bc88-ddf17f8e0f54">

### Container Image
A container image is a portable package that contains software. It's this image that, when run, becomes our container.

A container image is immutable. Once you've built an image, you can't change it. The only way to change an image is to create a new image. This feature is our guarantee that the image we use in production is the same image used in development and QA.

[Free Code Camp reference](https://www.freecodecamp.org/news/a-beginner-friendly-introduction-to-containers-vms-and-docker-79a9e3e119b/).

[Microsoft Learn reference](https://learn.microsoft.com/en-us/training/modules/intro-to-docker-containers/).

### VMs vs Containers
<img width="480" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/8731f9bc-291e-425d-b328-8d78103a07d7">

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

<img width="700" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/29cbde5c-6ad8-4ef6-8961-4f571f9414b7">

<img width="700" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/13325509-40fd-4b43-9b9f-f13e65b0f382">

Also take a look at [Microsoft Artifact Registry](https://mcr.microsoft.com/en-us/product/dotnet/samples/tags):

<img width="700" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/1d5335a5-a36d-4ded-9802-7d2dba8e2eaf">

### How Image is created
Go to this site:
https://hub.docker.com/_/microsoft-dotnet-samples/

<img width="500" alt="image" src="https://github.com/akhanalcs/k8s-hands-on/assets/30603497/bd9dfe43-2529-4cc3-a5d6-6eefaddcc3de">

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
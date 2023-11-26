# k8s-hands-on
This repo contains the notes I took when I studied courses at [Microsoft Learn](https://learn.microsoft.com/en-us/training/paths/intro-to-kubernetes-on-azure/), [YouTube](https://youtu.be/X48VuDVv0do?si=T1xGQVUEa2ZdTatH) and Andrew Lock's excellent [Kubernetes series](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-1-an-introduction-to-kubernetes/).

The notes from the theoretical portion of this learning are in docs folder which are linked as follows:
1. Microservices and Container basics. [Here](docs/basics-theory.md).
2. Hands on Docker course at Microsoft Learn. [Here](docs/docker-msftlearn.md).
3. Basic Kubernetes course at Microsoft Learn. [Here](docs/k8s-msftlearn.md).
4. Video course by Techworld at YouTube. [Here](docs/k8s-techworld.md).

The 'hands-on' portion of this learning is based on Andrew Lock's [Kubernetes series](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-1-an-introduction-to-kubernetes/). Feel free to look at the theoretical notes (1-4) if you're new to cloud native, otherwise jump straight into hands-on exercises below. You're going to learn a lot!

Also check out the following resources:
1. [9 tips](https://www.docker.com/blog/9-tips-for-containerizing-your-net-application/) for containerizing .NET apps.
2. ELI5 version of Kubernetes [video](https://youtu.be/4ht22ReBjno?si=gBkC4jhCS2G9ZYd5).
3. [Tips](https://mikehadlow.com/posts/2022-06-24-writing-dotnet-services-for-kubernetes/) using Kubernetes with .NET apps.

Happy Learning! 🤓

## Hands On Exercises
For this section, I'm following along Andrew Lock's [excellent series](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-1-an-introduction-to-kubernetes/) on Kubernetes.

As for theoretical aspect of Kubernetes, read the section after this one below.

### Create the projects
#### Clone this repo
#### Add a solution file using terminal
<img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/753af618-7cd5-4453-a993-28df78a8b90d">

Now open the solution.

#### Add a web api project to solution
1. Right Click Solution -> Add New Project -> Project name: `TestApp.Api`, Type: `Web API`

2. Add health check to it using guide [here](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0).  
   Check out the code to see how I implemented liveness and readiness checks.

3. Navigate to health check url  
   <img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/89c9c6be-b7c2-4816-90f5-ae5c360c7ab7">

4. Add an endpoint to expose environment info. I added a struct to return environment info. Check out to see how it's implemented.  
   For eg: This is what's returned when I run it in my Mac in Debug mode:  
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/62f764d0-92c9-4f59-8562-cf4f95fb4376">

   You can see that `memoryUsage` is 0 probably because `EnvironmentInfo` is written to extract this info when the app runs in Ubuntu. But I'm on a mac.

#### Add a console app
Create a CLI app for each of the main application.
This app will run migrations, take ad-hoc commands etc.

#### Add a service which is an empty web app
This is an empty web app. This app will run long running tasks using Background services, for eg: handling messages from event queue using something like NServiceBus or MassTransit. It easily could have been just a `Worker Service` but I kept it as a web app just so it's easier to expose health check endpoints.

<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5ff01df1-5772-4a9c-b948-12f1eddfc603">

Just has bare minimum code.

<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b0a337a4-9583-46b9-8e22-ef757d42b498">

We **won't expose** public HTTP endpoints for this app.

### Add Dockerfile to all 3 projects
Add Dockerfile by following this [guide](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-8.0).

Check out these **EXCELLENT** samples: https://github.com/dotnet/dotnet-docker/tree/main/samples/aspnetapp

Learn about Chiseled containers [here](https://andrewlock.net/exploring-the-dotnet-8-preview-updates-to-docker-images-in-dotnet-8/#support-for-chiseled-containers).

### Create images
Go to the directory where the Dockerfile is in the terminal and run these commands to create the images.

````
docker build -f TestApp.Api.Dockerfile -t akhanal/test-app-api:0.1.0 .
docker build -f TestApp.Service.Dockerfile -t akhanal/test-app-service:0.1.0 .
docker build -f TestApp.Cli.Dockerfile -t akhanal/test-app-cli:0.1.0 .
````

The last parameter `.` is the build context. This means that the `.` used in the Dockerfile refers to `.` parameter which is current directory.

For eg:  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/70d37a34-5d61-4123-b303-8180dae572a1">

Here `.` in "./TestApp.Api/TestApp.Api.csproj" in Dockerfile just means the directory given by the build context parameter.

View the created images:
````
docker images "akhanal/*"
````
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1dadd287-f4e0-4c92-8e68-bdc0a42cf6fb">

### Test out the image
Remove the `http` profile from `launchSettings.json` file.  
And run this:
````
docker run --rm -it -p 8000:8080 -e ASPNETCORE_ENVIRONMENT=Development akhanal/test-app-api:0.1.0
````
<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8f52aa03-ea27-4681-ad22-b5c1ff11de12">

The container only exposes `http` here.  
To expose `https`, we need to add certificate.

One thing to note here is that [aspnetcore apps from .NET 8 use port 8080 port by default](https://andrewlock.net/exploring-the-dotnet-8-preview-updates-to-docker-images-in-dotnet-8/#asp-net-core-apps-now-use-port-8080-by-default).

[Reference](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-8.0).

### Install Kubernetes
Make sure you have docker desktop installed and enable Kubernetes on it.

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e8eae263-6f83-4b67-a915-a509a7093de5">

#### Enable Kubernetes dashboard
Follow instructions [here](https://kubernetes.io/docs/tasks/access-application-cluster/web-ui-dashboard/).

````
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
````
You can enable access to the Dashboard using the kubectl command-line tool, by running the following command:
````
kubectl proxy
````
Kubectl will make Dashboard available at:  
http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/

Now read this [blog post](https://andrewlock.net/running-kubernetes-and-the-dashboard-with-docker-desktop/) to disable the login prompt.
Or if you want to create a user to login, follow this [tutorial](https://medium.com/@dijin123/kubernetes-and-the-ui-dashboard-with-docker-desktop-5ad4799b3b61).

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ca3a9481-7f06-4927-9704-bf2e81d29879">

Now run `kubectl proxy` and go to the dashboard url, and hit "Skip" on the login screen.

#### Fix permission issues
At this point, you'll only be able to view default namespace and see a bunch of errors in the notification.

<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bffd1281-0bf5-471b-a05c-3b11a561bf2d">

The fix for that is giving `cluster-admin` role to `system:serviceaccount:kubernetes-dashboard:kubernetes-dashboard` user like so:
````
$ kubectl delete clusterrolebinding serviceaccount-cluster-admin
$ kubectl create clusterrolebinding serviceaccount-cluster-admin --clusterrole=cluster-admin --user=system:serviceaccount:kubernetes-dashboard:kubernetes-dashboard
````

Now restart `kubectl proxy` and refresh the browser.

### Install helm chart
Follow instructions here: https://helm.sh/docs/intro/install/

I used Homebrew to install it in my Mac
````
brew install helm
````

### Create helm chart
Add a folder at the solution level named `charts`.

Go into the folder and create a new chart called `test-app`.

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1227ce3e-fd6d-421f-835b-968f43f32059">

Remove templates folder  
<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a013c80f-20fa-476d-84de-d5dc55a6d752">
<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/16a2267b-030e-4ed4-9896-83a130b5547f">

Now go into `charts` folder and create charts for TestApp.Api and TestApp.Service
````
helm create test-app-api # Create a sub-chart for the API
helm create test-app-service # Create a sub-chart for the service
````

Remove these files for sub charts
````
rm test-app-api/.helmignore test-app-api/values.yaml
rm test-app-service/.helmignore test-app-service/values.yaml
````

Also remove these files for sub charts
````
rm test-app-api/templates/hpa.yaml test-app-api/templates/serviceaccount.yaml
rm test-app-service/templates/hpa.yaml test-app-service/templates/serviceaccount.yaml
rm -r test-app-api/templates/tests test-app-service/templates/tests
````

Now the folder structure looks like this:

<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/734efd04-e753-469e-889c-e2a53f4f3889">

This structure treats projects in this solution to be microservices that are deployed at the same time.
So this solution is a "microservice" here.

If you change a sub-chart, you have to bump the version number of that **and** the top level chart. Annoying though!

We use top level `values.yaml` to share config with the sub charts as well.

**Tip:** Don't include `.` in your chart names, and use lower case. It just makes everything easier.

#### Looking around the templates
<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ac01cde6-d0a4-4fa6-9c5c-f3af01de47ae">

(About this `nindent`, you can figure out the indentation number by sitting where you want the text to sit, and going left. 
For eg: I had to hit left arrow 8 times until I reached the start of this line, so indent value is 8 here.)

In Helm, the `{{- with .Values.imagePullSecrets }}` statement is a control structure that sets the context to `.Values.imagePullSecrets`. The `-` character in `{{- with` is used to trim whitespace.

The `imagePullSecrets:` line specifies any image pull secrets that may be required to pull the container images.

The `{{- toYaml . | nindent 8 }}` line is doing two things:
1. `toYaml .` is converting the current context (which is `.Values.imagePullSecrets` due to the `with` statement) to YAML.
2. `nindent 8` is indenting the resulting YAML by 8 spaces.

The `{{- end }}` statement ends the with block.

So, this whole block is checking if `.Values.imagePullSecrets` is set, and if it is, it’s adding an `imagePullSecrets` field to the Pod spec with the value of `.Values.imagePullSecrets`, converted to YAML and indented by 8 spaces.

For example, if your `values.yaml` file contains:
````
imagePullSecrets:
  - name: myregistrykey
````
Then the resulting spec would be:
````
    spec:
      imagePullSecrets:
        - name: myregistrykey
````
If `values.yaml` doesn't contain that, `imagePullSecrets` won't appear in the resulting `spec`.

### Install Ingress controller
Follow instructions [here](https://kubernetes.github.io/ingress-nginx/deploy/#docker-desktop) for Docker Desktop Kubernetes environment.

````
helm upgrade --install ingress-nginx ingress-nginx \
  --repo https://kubernetes.github.io/ingress-nginx \
  --namespace ingress-nginx --create-namespace
````

A pod will be deployed which you can check:
````
kubectl -n ingress-nginx get pod -o yaml
````

The information you need from this controller is `ingressClassName` which you'll put it in your `values.yaml` file, which will eventually make it to `ingress.yaml` file.

Find the `ingressClassName` of your controller by either running this command: `kubectl get ingressclasses` or finding it through K8s dashboard.

Command way:  
<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0b83d35c-91fa-4b16-9a71-3cb1ef87b877">

Dashboard way:  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2fd777e8-6b6b-4944-94c5-e5041764ce7f">

### Liveness, Readiness and Startup probes
[Reference](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-6-adding-health-checks-with-liveness-readiness-and-startup-probes/)

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a12ffa29-4da0-4356-9b27-69293ac3a28d">

#### Startup Probe
The first probe to run is the startup probe.
As soon as the startup probe succeeds once it never runs again for the lifetime of that container. If the startup probe never succeeds, Kubernetes will eventually kill the container, and restart the pod.

#### Liveness Probe
The liveness probe is what you might expect—it indicates whether the container is alive or not. If a container fails its liveness probe, Kubernetes will kill the pod and restart another.

Liveness probes happen continually through the lifetime of your app.

#### Readiness Probe
Readiness probes indicate whether your application is ready to handle requests. It could be that your application is alive, but that it just can't handle HTTP traffic. In that case, Kubernetes won't kill the container, but it will stop sending it requests. In practical terms, that means the pod is removed from an associated service's "pool" of pods that are handling requests, by marking the pod as "Unready".

Readiness probes happen continually through the lifetime of your app, exactly the same as for liveness probes.

### Types of health checks
- **Smart** probes typically aim to verify the application is working correctly, that it can service requests, and that it can connect to its dependencies (a database, message queue, or other API, for example).
- **Dumb** health checks typically only indicate the application has not crashed. They don't check that the application can connect to its dependencies, and often only exercise the most basic requirements of the application itself i.e. can they respond to an HTTP request.

#### Use smart startup probes
#### Use dumb liveness probes to avoid cascading failures
#### Use dumb readiness probes

### Update the chart for my apps
#### Update `values.yaml`
The config for `test-app-api` looks like below (not showing the config for `test-app-service` here. Check out the code to see the whole thing):
````
test-app-api: 
  replicaCount: 1

  image:
    repository: akhanal/test-app-api
    pullPolicy: IfNotPresent
    # Overrides the image tag whose default is the chart appVersion.
    # We'll set a tag at deploy time
    tag: ""

  service:
    type: ClusterIP
    port: 80
      
  ingress:
    enabled: true
    # How to find this value is explained in section right above.
    className: nginx
    annotations:
      # Reference: https://kubernetes.github.io/ingress-nginx/examples/rewrite/
      nginx.ingress.kubernetes.io/use-regex: "true"
      nginx.ingress.kubernetes.io/rewrite-target: /$2
    hosts:
      - host: chart-example.local
        paths:
          - path: /my-test-app(/|$)(.*)
            pathType: ImplementationSpecific

  autoscaling:
    enabled: false

  serviceAccount:
    # Specifies whether a service account should be created
    create: false
````
I didn't specify the image tag as I'll specify that at deploy time.

#### Update container port in `deployment.yaml`
Recall that aspnetcore apps now [run on port 8080 by default](https://andrewlock.net/exploring-the-dotnet-8-preview-updates-to-docker-images-in-dotnet-8/#asp-net-core-apps-now-use-port-8080-by-default). So we have to update the container port in `deployment.yaml` file.

<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d9e4fedd-f954-4777-b824-0cb654a49e6f">

#### Update startup, liveness and readiness checks in `deployment.yaml`

### Deploying to Kubernetes
Now go to `charts/test-app` folder in terminal (because we have `Chart.yaml` there) and run the following command:

This creates (or upgrades an existing release) using the name `test-app-release`.
````
helm upgrade --install test-app-release . \
--namespace=local \
--set test-app-api.image.tag="0.1.0" \
--set test-app-service.image.tag="0.1.0" \
--create-namespace \
--debug \
--dry-run
````
(When writing a command over multiple lines, make sure there's no space after the backslash and before the newline.)

Specifies that everything should be created in the `local` namespace of Kubernetes cluster.

`--dry-run` means we don't actually install anything. Instead, Helm shows you the manifests that would be generated, so you can check everything looks correct.

This is the manifest that gets created for `test-app-api` which shows the creation of `Service`, `Deployment` and `Ingress`:
````
# Source: test-app/charts/test-app-api/templates/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: test-app-release-test-app-api
  labels:
    helm.sh/chart: test-app-api-0.1.0
    app.kubernetes.io/name: test-app-api
    app.kubernetes.io/instance: test-app-release
    app.kubernetes.io/version: "1.16.0"
    app.kubernetes.io/managed-by: Helm
spec:
  type: ClusterIP
  ports:
    - port: 80
      targetPort: http
      protocol: TCP
      name: http
  selector:
    app.kubernetes.io/name: test-app-api
    app.kubernetes.io/instance: test-app-release
---
# Source: test-app/charts/test-app-api/templates/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: test-app-release-test-app-api
  labels:
    helm.sh/chart: test-app-api-0.1.0
    app.kubernetes.io/name: test-app-api
    app.kubernetes.io/instance: test-app-release
    app.kubernetes.io/version: "1.16.0"
    app.kubernetes.io/managed-by: Helm
spec:
  replicas: 1
  selector:
    matchLabels:
      app.kubernetes.io/name: test-app-api
      app.kubernetes.io/instance: test-app-release
  template:
    metadata:
      labels:
        helm.sh/chart: test-app-api-0.1.0
        app.kubernetes.io/name: test-app-api
        app.kubernetes.io/instance: test-app-release
        app.kubernetes.io/version: "1.16.0"
        app.kubernetes.io/managed-by: Helm
    spec:
      serviceAccountName: default
      securityContext:
        null
      containers:
        - name: test-app-api
          securityContext:
            null
          image: "akhanal/test-app-api:0.1.0"
          imagePullPolicy: IfNotPresent
          ports:
            - name: http # This name is referenced in service.yaml
              containerPort: 8080
              protocol: TCP
          livenessProbe:
            httpGet:
              path: /healthz/live
              port: http
          readinessProbe:
            httpGet:
              path: /healthz/ready
              port: http
            # My container has startup time (simulated) of 15 seconds, so I want readiness probe to run only after 20 seconds.
            initialDelaySeconds: 20
          resources:
            null
---
# Source: test-app/charts/test-app-api/templates/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: test-app-release-test-app-api
  labels:
    helm.sh/chart: test-app-api-0.1.0
    app.kubernetes.io/name: test-app-api
    app.kubernetes.io/instance: test-app-release
    app.kubernetes.io/version: "1.16.0"
    app.kubernetes.io/managed-by: Helm
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /$2
    nginx.ingress.kubernetes.io/use-regex: "true"
spec:
  ingressClassName: nginx
  rules:
    - host: "chart-example.local"
      http:
        paths:
          - path: /my-test-app(/|$)(.*)
            pathType: ImplementationSpecific
            backend:
              service:
                name: test-app-release-test-app-api
                port:
                  number: 80
````

Now run the above command without the `--dry-run` flag which will deploy the chart to Kubernetes cluster.  
<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/571ba762-7c77-4efe-b3b0-aba53de63115">

The deployed resources will look like this:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5a937a43-dafa-4b01-979d-f3d198465052">

Note that this is the command to uninstall the app
````
 helm uninstall test-app-release -n local
````

### Update hosts file
Check the ingress you deployed to see what address was assigned to your host because you'll be using that address to update your hosts file.
````
kubectl get ingress -n local
````

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4dbed22d-9601-41e1-8d3b-b1890a49ccdc">

Also seen in controller logs:
````
W1119 05:14:31.194021       7 controller.go:1214] Service "local/test-app-release-test-app-api" does not have any active Endpoint.
I1119 05:15:19.437846       7 status.go:304] "updating Ingress status" namespace="local" ingress="test-app-release-test-app-api" currentValue=null newValue=[{"hostname":"localhost"}]
````

Now add this mapping to hosts file.

````
sudo vim /etc/hosts
````

Enter the server IP address at the bottom of the hosts file, followed by a space, and then the domain name.

<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d98b2441-9a0d-4f08-a4ad-f2f75386e99b">

Save and exit with `:wq`.

Verify your changes with
````
cat /etc/hosts
````

Now, you should be able to reach the app using:  
http://chart-example.local/my-test-app/weatherforecast

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e0d04a8b-48ce-4a82-8bf9-2773320690b6">

### Troubleshooting pods restarting (only here for learning exercise, the issue is not present in the example app in this repo)
Check out the pods.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f136f42c-e34d-456d-96d6-b351279f2dd5">

You can see that they haven't been able to get ready and have already restarted many times.

Check out the reason why the Pods were restarted so often by looking at Pod's events:
````
kubectl get event -n local --field-selector involvedObject.name=test-app-release-test-app-api-97757b99b-ppx9g
````

<img width="950" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/725e1728-bb47-4e09-be34-74718f0b99e9">

We can see that the containers were restarted because the readiness probe failed.

Or you can view this info in the Kubernetes dashboard:

<img width="950" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f93701c1-868d-48c0-84f9-721bbcdf9847">

The issue here is that it's trying to hit the wrong port (i.e. 80). Recall that the aspnet core apps use 8080 port by default. 

The port the container has started on (8080) can be viewed from the pod logs as well:

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/3b212aa1-2ccf-4a33-8ef4-0b6cc5a91de4">

To fix this, we have to update `containerPort` in `deployment.yaml`:

<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/88021838-1741-4c00-b020-52ac4f024475">

### Troubleshooting Ingress not working (only here for learning exercise, the issue is not present in the example app in this repo)
#### Issue 1: `chart-example.local` hostname doesn't get an address
````
kubectl get ingress -n local
````
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d70bf0d5-5499-4b0c-be14-872a4b0975fa">

When this happens, you don't know what address is assigned by ingress controller for the host name, so you won't be able to add this entry to your hosts file.

Jump into logs of Ingress controller from the K8s dashboard.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e8b97e23-7de5-4d31-a7f4-17a30f6524b4">

This is the error seen in the logs:
````
"Ignoring ingress because of error while validating ingress class" ingress="local/test-app-release-test-app-api" error="ingress does not contain a valid IngressClass"
````

Change this:
````
  ingress:
    enabled: true
    annotations:
      kubernetes.io/ingress.class: nginx
````
to this:
````
  ingress:
    enabled: true
    # Find the classname of your controller by running this command: `kubectl get ingressclasses` or find it through K8s dashboard
    className: nginx
````

Summary: The fix is to remove the `ingress.class` annotation and add ingress `className`.

#### Issue 2: The service always returns 404
Navigating to the url: http://chart-example.local/my-test-app/weatherforecast returns 404. This is a 404 returned by the app (not the nginx controller), so you can see that the app is reachable. This should tell you that the issue is in routing.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/03d7e7c9-3783-4da3-af96-43b2f505bcb6">

Change the rewrite target from this:
````
    annotations:
      nginx.ingress.kubernetes.io/rewrite-target: "/"
    hosts:
      - host: chart-example.local
        paths:
          - path: "/my-test-app"
            pathType: ImplementationSpecific
````
to this:
````
    annotations:
      # Reference: https://kubernetes.github.io/ingress-nginx/examples/rewrite/
      nginx.ingress.kubernetes.io/use-regex: "true"
      nginx.ingress.kubernetes.io/rewrite-target: /$2
    hosts:
      - host: chart-example.local
        paths:
          - path: /my-test-app(/|$)(.*)
            pathType: ImplementationSpecific
````

[Reference](https://kubernetes.github.io/ingress-nginx/examples/rewrite/)

### Configure aspnetcore apps to work with proxy servers and load balancers
[Reference](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-8.0)

- When HTTPS requests are proxied over HTTP, the original scheme (HTTPS) is lost and must be forwarded in a header. This is SSL/ TLS offloading.
- Because an app receives a request from the proxy and not its true source on the Internet or corporate network, the originating client IP address must also be forwarded in a header.

Forwarded headers middleware is enabled by setting an environment variable.
````
ASPNETCORE_FORWARDEDHEADERS_ENABLED = true
````

### Setting environment variables
[Reference](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-5-setting-environment-variables-in-a-helm-chart/)

Environment variables are set in `deployment.yaml` file.   
Rather than hardcoding values and mappings in `deployment.yaml` file, it's better to use Helm's templating capabilities to extract this into configuration. 

`deployment.yaml`
````
env:
{{ range $k, $v := .Values.global.envValuesFrom }} # dynamic values
  - name: {{ $k | quote }}
    valueFrom:
      fieldRef:
        fieldPath: {{ $v | quote }}
{{- end }}

{{- $env := merge (.Values.env | default dict) (.Values.global.env | default dict) -}} # static values, merged together
{{ range $k, $v := $env }}
  - name: {{ $k | quote }}
    value: {{ $v | quote }}
{{- end }}
````

`values.yaml`
````
global:
  # Dynamic values
  # Environment variables shared between all the pods, populated with valueFrom: fieldRef
  # Reference: https://kubernetes.io/docs/tasks/inject-data-application/environment-variable-expose-pod-information/
  envValuesFrom:
    Runtime__IpAddress: status.podIP

  # Static values
  env: 
    "ASPNETCORE_ENVIRONMENT": "Staging"
    "ASPNETCORE_FORWARDEDHEADERS_ENABLED": "true"
````

Note that I've used the double underscore `__` in the environment variable name. The translates to a "section" in ASP.NET Core's configuration, so this would set the configuration value `Runtime:IpAdress` to the pod's IP address.

At install time, we can override these values if we like.
````
helm upgrade --install my-test-app-release . \
  --namespace=local \
  --set test-app-api.image.tag="0.1.0" \
  --set test-app-service.image.tag="0.1.0" \
  --set global.env.ASPNETCORE_ENVIRONMENT="Development" \          # global value
  --set test-app-api.env.ASPNETCORE_ENVIRONMENT="Staging"  # sub-chart value
````
I can view my environment variables!

<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/130a4266-84f5-480d-ac18-41a24f6dffcc">

### Running database migrations
[Reference](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-7-running-database-migrations/)

Use Kubernetes Jobs and Init containers.

#### Jobs
A Kubernetes job executes one or more pods to completion, optionally retrying if the pod indicates it failed, and then completes when the pod exits gracefully.
We can create a job that executes a simple .NET core console app, optionally retrying to handle transient network issues.

Now go into `charts` folder and create a new chart for `TestApp.Cli`. I was wondering if helm had a different command for jobs, but [looks like it doesn't](https://stackoverflow.com/q/69837824/8644294). So, I went down the path of creating a chart for an app and removing things I didn't need.
````
helm create test-app-cli #Create a sub-chart for the Cli
````

Remove these files for `test-app-cli` sub chart
````
rm test-app-cli/.helmignore test-app-cli/values.yaml
rm test-app-cli/templates/hpa.yaml test-app-cli/templates/serviceaccount.yaml
rm test-app-cli/templates/ingress.yaml test-app-cli/templates/NOTES.txt
rm test-app-cli/templates/service.yaml test-app-cli/templates/deployment.yaml
rm -r test-app-cli/templates/tests
rm -r test-app-cli/charts
````

Add a new file to `test-app-cli/templates/job.yaml`.

Start off with this, and create a Job resource:  
<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cd25bf36-6273-4704-aff7-c5d913d996d2">

Or just copy an example of a job from the Kubernetes docs [site](https://kubernetes.io/docs/concepts/workloads/controllers/job/).

And edit the file to look like this:
````
apiVersion: batch/v1
kind: Job
metadata:
  name: {{ include "test-app-cli.fullname" . }}-{{ .Release.Revision }}
  labels:
    {{- include "test-app-cli.labels" . | nindent 4 }}
spec:
  backoffLimit: 1
  template:
    metadata:
      labels:
        {{- include "test-app-cli.selectorLabels" . | nindent 8 }}
    spec:
      restartPolicy: {{ .Values.job.restartPolicy }}
      containers:
        - name: {{ .Chart.Name }}
          image: "{{ .Values.image.repository }}:{{ .Values.image.tag | default .Chart.AppVersion }}"
          imagePullPolicy: {{ .Values.image.pullPolicy }}
          command: [ "dotnet" ]
          args: [ "TestApp.Cli.dll", "migrate-database" ]
          env:
          # Dynamic environment values
          {{ range $k, $v := .Values.global.envValuesFrom }}
            - name: {{ $k | quote }}
              valueFrom:
                fieldRef:
                  fieldPath: {{ $v | quote }}
          {{- end }}
          # Static environment variables
          {{- $env := merge (.Values.env | default dict) (.Values.global.env | default dict) -}} # Static values merged together with global values taking non-priority if specific env values are provided.
          {{ range $k, $v := $env }}
            - name: {{ $k | quote }}
              value: {{ $v | quote }}
          {{- end }}
````

Now pass the config values from top level `values.yaml`
````
test-app-cli:
  image:
    repository: akhanal/test-app-cli # Make sure that you have docker image of the Cli project
    pullPolicy: IfNotPresent
    tag: ""

  job:
    # Should the job be rescheduled on the same node if it fails, or just stopped
    restartPolicy: Never
````

Test the job
````
helm upgrade --install test-app-release . --namespace=local --set test-app-cli.image.tag="0.1.0" --set test-app-api.image.tag="0.1.0" --set test-app-service.image.tag="0.1.0"
````

Check it out in the dashboard:

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/694b33a0-193b-43c5-8270-96162e1a91f1">

Also view the logs:

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ab63ad54-4fea-4306-857c-f5b7f20552fe">

Note that we haven't implemented init containers yet, so our application pods will immediately start handling requests **without** waiting for the job to finish.

#### Use Init Containers to delay container startup
Init containers are a special type of container in a pod. When Kubernetes deploys a pod, it runs all the init containers first. Only once all of those containers have exited gracefully will the main containers be executed. Init containers are often used for downloading or configuring pre-requisites required by the main container. That keeps your container application focused on it's one job, instead of having to configure it's environment too.

In this case, we're going to use init containers to watch the status of the migration job. The init container will sleep while the migration job is running (or if it crashes), blocking the start of our main application container. Only when the job completes successfully will the init containers exit, allowing the main container to start.

We can use a Docker container containing the _k8s-wait-for_ script, and include it as an init container in all our application deployments.

Add this to a section before containers in `test-app-cli` and `test-app-service`
````
      initContainers:
        - name: "{{ .Chart.Name }}-init" # test-app-api-init will be the name of this container
          image: "groundnuty/k8s-wait-for:v2.0"
          imagePullPolicy: {{ .Values.image.pullPolicy }}
          # WAIT for a "job" with a name of "test-app-release-test-app-cli-1"
          args:
            - "job"
            - "{{ .Release.Name }}-test-app-cli-{{ .Release.Revision }}" # This is the name defined in job.yaml -> metadata:name
      containers:
        - name: {{ .Chart.Name }}
        # Other config here
````

Now deploy the app
````
helm upgrade --install test-app-release . --namespace=local --set test-app-cli.image.tag="0.1.0" --set test-app-api.image.tag="0.1.0" --set test-app-service.image.tag="0.1.0"
````

Note that this is the command to uninstall the app
````
 helm uninstall test-app-release -n local
````

**This is what's happening here:**

The Kubernetes job runs a single container that executes the database migrations as part of the Helm Chart installation. Meanwhile, init containers in the main application pods prevent the application containers from starting. Once the job completes, the init containers exit, and the new application containers can start.

<img width="450" alt="image" src="https://andrewlock.net/content/images/2020/jobs-and-init-containers.svg">

#### Troubleshooting init container failing
This is the error seen right after deployment:  
<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8ccf7e5f-34c5-4110-b54a-85a04681de18">

Now let's check init container logs by going into Pod -> clicking Logs -> selecting init container.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a2818d40-1995-49d0-9aa6-ba7d90331fda">

Or you can use `kubectl` to get the container logs. For eg:
````
kubectl logs test-app-release-test-app-api-d75cfd5c9-jmrjw -c test-app-api-init -n local
````
This shows the error we're facing.
````
Error from server (Forbidden): jobs.batch "test-app-release-test-app-cli-1" is forbidden: User "system:serviceaccount:local:default" cannot get resource "jobs" in API group "batch" in the namespace "local"
````

This means the pod lacks the permissions to perform `kubectl get` query. [Reference](https://github.com/groundnuty/k8s-wait-for#troubleshooting).

The fix for this is to create a role that has permission to read jobs, and bind that role to the `default` service account (`local:default`) in the `local` namespace. The `--serviceaccount` flag should be in the format `<namespace>:<serviceaccount>`.

1. Create the Role
   ````
   kubectl create role job-reader --verb=get --verb=list --verb=watch --resource=jobs --namespace=local
   ````
2. Create the RoleBinding
   ````
   # This role binding allows "local:default" service account to read jobs in the "local" namespace.
   # You need to already have a role named "job-reader" in that namespace.
   kubectl create rolebinding read-jobs --role=job-reader --serviceaccount=local:default --namespace=local
   ````
<img width="950" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/43859dc3-dcd7-4507-afe7-58524a34fb2a">

This fixes the problem!

#### Test init container working 🎉
When the `cli` job is running, the status of our main app is `Init: 0/1`.

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e153b84b-1ff3-47ec-9c80-5d6e48f674d8">

After the job gets Completed, our app starts Running. 💪

<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/40f6f17a-8068-48ef-b9ba-4b27983c1a77">

### Monitoring Helm Releases
[Reference](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-9-monitoring-helm-releases-that-use-jobs-and-init-containers/)

Helm doesn't know about our "delayed startup" approach. Solution is to wait for a Helm release to complete.

Add this file.  
And give execute permissions to the file using `chmod +x ./deploy_and_wait.sh` by going to the folder where it's at.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/7fb38fda-bfc0-4285-aa7b-00a9e0636848">

Now run the script
````
CHART="test-app-repo/test-app" \
RELEASE_NAME="test-app-release" \
NAMESPACE="local" \
HELM_ARGS="--set test-app-cli.image.tag=0.1.0 \
  --set test-app-api.image.tag=0.1.0 \
  --set test-app-service.image.tag=0.1.0 \
" \
./deploy_and_wait.sh
````

I got this error:
````
Error: repo test-app-repo not found
````

I didn't bother with creating a Helm repository and moved on to next post.

### Creating 'exec-host' deployment for running one-off commands
[Reference](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-10-creating-an-exec-host-deployment-for-running-one-off-commands/)
By using a long-running pod containing a CLI tool that allows running the commands.

#### Create Dockerfile for TestApp.Cli-Exec-Host
We can use the exisiting CLI project, i.e. `TestApp.Cli` to create an image for this.

After you're done creating the Dockerfile, build it
````
docker build -f TestApp.Cli-Exec-Host.Dockerfile -t akhanal/test-app-cli-exec-host:0.1.0 .
````

Now create helm chart for this app.
````
helm create test-app-cli-exec-host
````
Delete all files except `Chart.yaml`, `templates/_helpers.tpl` and `templates/deployment.yaml`. From `deployment.yaml`, remove liveness/ readiness checks, and ports.  
And add a section for injecting env variables.

Add `test-app-cli-exec-host` config to top-level chart's `values.yaml` to specify docker image and some other settings.

At this point, our overall Helm chart has now grown to 4 sub-charts: The two "main" applications (the API and message handler service), the CLI job for running database migrations automatically, and the CLI exec-host chart for running ad-hoc commands

Install the chart
````
helm upgrade --install test-app-release . \
--namespace=local \
--set test-app-api.image.tag="0.1.0" \
--set test-app-service.image.tag="0.1.0" \
--set test-app-cli.image.tag="0.1.0" \
--set test-app-cli-exec-host.image.tag="0.1.0" \
--create-namespace \
--debug
````

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b07c0446-340e-41e9-9ca0-18bb07397f26">

Try getting into the container by clicking this:  
<img width="200" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/664bdb82-ee4e-4a79-b1c8-a31d312540b7">

We have access to our CLI tool from here and can run ad-hoc commands from the cli app.😃 For eg:

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/328de7c4-cabc-491c-ab7b-26bf53bd63a9">

Remember that it comes from the CLI program.  
<img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/22c66d9c-f5ab-464e-947d-f92a36e0f995">


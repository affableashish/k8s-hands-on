# k8s-hands-on
This repo contains notes from Docker and Kubernetes course at [Microsoft Learn](https://learn.microsoft.com/en-us/training/paths/intro-to-kubernetes-on-azure/) and also [Kubernetes course](https://youtu.be/X48VuDVv0do?si=T1xGQVUEa2ZdTatH) by TechWorld with Nana.

Also check out the following resources:
1. [9 tips](https://www.docker.com/blog/9-tips-for-containerizing-your-net-application/) for containerizing .NET apps.
2. ELI5 version of Kubernetes [video](https://youtu.be/4ht22ReBjno?si=gBkC4jhCS2G9ZYd5).
3. Kubernetes [series](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-1-an-introduction-to-kubernetes/) by Andrew Lock.
4. [Best practices](https://mikehadlow.com/posts/2022-06-24-writing-dotnet-services-for-kubernetes/) using Kubernetes with .NET apps.

## Hands On Example
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

## Kubernetes
Reference: [Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/) and [this YouTube video](https://youtu.be/X48VuDVv0do?si=UqYr2D-Lv_aL9bXc).

Kubernetes is a portable, extensible open source platform for management and orchestration of containerized workloads.

### Container management
It is the process of organizing, adding, removing or updating a significant number of containers.

### Container Orchestrator
It is a system that automatically deploys and manages containerized apps. As part of management, it handles scaling dynamic changes in the environment to increase or decrease the number of deployed instances of the app.  
It also ensures that all deployed container instances are updated when a new version of a service is released.

<img width="700" alt="image" src="https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/2-tasks-of-orchestrator.svg">

### Kubernetes Benefits
<img width="600" alt="image" src="https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/media/2-kubernetes-benefits.svg">

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

Image taken from [here](https://kubernetes.io/docs/concepts/overview/components/).

#### Cluster
Cluster is a set of computers that you configure to work together and view as a single system. The computers configured in the cluster handle same kind of tasks. For eg, they all host websites, APIs or run compute intensive works.

A cluster uses centralized software that's responsible for scheduling and controlling these tasks.  
The computers in a cluster that run the tasks are called nodes, and the computers that run the scheduling software are called control planes.  
<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4e3a1c69-51da-4695-9c12-a2845f02f168">

#### Kubernetes Architecture
You use Kubernetes as the orchestration and cluster software to deploy your apps and respond to changes in compute resource needs.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e1202f9e-e735-42db-974a-173445306e7c">

A K8s cluster contains at least one main plane and one or more nodes. THe default host OS in K8s is Linux, with default support for Linux based workloads.<br>
You can also run Microsoft workloads by using Windows Server 2019 or later on cluster nodes. For eg: if you have some app that's written in `.NET 4.5`, this can run only on nodes that run a Windows Server OS.

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

**Test environment** -> 1 control plane  
**Prod environment** -> 3-5 controls planes

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
1. A user or a controller schedule a new pod by submitting it to the Kubernetes API server.
2. The API server adds the new pod to its store of Kubernetes objects and makes it available to the scheduler.
3. The scheduler watches for newly created pods that have no node assigned. This is what is meant by “monitoring newly created containers”.
4. For each new pod, the scheduler determines which nodes have enough free resources to run the pod.
5. The scheduler then ranks each suitable node based on its scheduling algorithm and assigns the pod to the best node.
6. Once the scheduler has made its decision, it notifies the API server, which in turn notifies the chosen node.
   
So, the scheduler doesn’t create containers or pods. It simply decides where new pods should run based on the current state of the cluster and the resource requirements of the new pod. The actual creation and management of containers within a pod is handled by the `Kubelet` on the chosen node.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5304b47a-6e3d-42ab-b692-17fe30a55308">

#### Controller manager
Controller manager launches and monitors the controllers configured for a cluster through the API server. "through the API server" means that the controller manager uses the API server to interact with the K8s objects that the controllers manage.  

1. Track Object States:  
   Kubernetes uses controllers to track object states in the cluster. Each controller runs in a non-terminating loop, watching and responding to events in the cluster. For example, there are controllers to monitor nodes, containers, and endpoints.

2. Communicate with the API Server:  
   The Controller communicates with the API server to determine the object's state. It checks if the current state of an object is different from its desired state.

3. Ensure Desired State:  
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

<img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0d043a3f-019c-46b8-9a97-f947207a8f9c">

The following services run on the Kubernetes node:

1. Kubelet
2. Kube-proxy
3. Container runtime

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

### Interact with a Kubernetes cluster
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a2adfddb-e6f5-4bac-ac23-540d381b32a7">

K8s provides a command line tool called `kubectl` to manage your cluster. You use `kubectl` to send commands to the cluster's control plane or fetch information about all K8s objects via the API server.

`kubectl` uses a configuration file that includes the following configuration information:

1. **Cluster** configuration specifies a cluster name, certificate information, and the service API endpoint associated with the cluster. This definition allows you to connect from a single workstation to multiple clusters.
2. **User** configuration specifies the users and their permission levels when they're accessing the configured clusters.
3. **Context** configuration groups clusters and users by using a friendly name. For example, you might have a "dev-cluster" and a "prod-cluster" to identify your development and production clusters.

You can configure `kubectl` to connect to multiple clusters by providing the correct context as part of the command-line syntax.

### Kubernetes Pods
A pod represents a single instance of an app running in K8s.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cad0406a-8b59-4282-b58f-1c973ddd4884">

Unlike in a Docker environment, you can't run containers directly on Kubernetes. You package the container into a Kubernetes object called a pod. A pod is the smallest object that you can create in Kubernetes.

A single pod can hold a group of one or more containers. However, a pod typically doesn't contain multiples of the same app.

A pod includes information about the shared storage and network configuration and a specification about how to run its packaged containers. You use pod templates to define the information about the pods that run in your cluster. Pod templates are YAML-coded files that you reuse and include in other objects to manage pod deployments.

For example, let's say that you want to deploy a website to a Kubernetes cluster. You create the pod definition file that specifies the app's container images and configuration. Next, you deploy the pod definition file to Kubernetes.

Assume that your site uses a database. The website is packaged in the main container, and the database is packaged in the supporting container. Multiple containers communicate with each other through an environment. The containers include services for a host OS, network stack, kernel namespace, shared memory, and storage volume. The pod is the sandbox environment that provides all of these services to your app. The pod also allows the containers to share its assigned IP address.

<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c7016ccb-02df-4396-afa1-1c53cd665787">

Because you can potentially create many pods that are running on many nodes, it can be hard to identify them. You can recognize and group pods by using string labels that you specify when you define a pod.

#### Lifecycle of Kubernetes Pods
Kubernetes pods have a distinct lifecycle that affects the way you deploy, run, and update pods. You start by submitting the pod YAML manifest to the cluster. After the manifest file is submitted and persisted to the cluster, it defines the desired state of the pod. The scheduler schedules the pod to a healthy node that has enough resources to run the pod.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/34330559-426b-433f-9790-81c8b0b150e9">

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

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c955a7b0-f0ea-45e6-b031-d548e70ea8cb">

#### Pod Deployment options
There are several options to manage the deployment of pods in a Kubernetes cluster when you're using `kubectl`. The options are:

1. Pod templates
2. Replication Controllers
3. Replica set
4. Deployments

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

**Note:** Databases can't be replicated via deployment because Db have state. If we have clones or replica of the database, they would all need to access the same shared data storage and they you would need some kind of mechanism that manages which pods are currently reading/ writing from/ to that storage in order to avoid data inconsistencies and that mechanism in addition to replicating feature is offered by another K8s component called **StatefulSet**. This component is meant specifically for databases.

Deploying StatefulSet is not easy that's why Databases are often hosted outside the K8s cluster.

**Layers of Abstraction:**

Everything below Deployment is handled by K8s.

| **DEPLOYMENT manages a 👇** |
| --- |
| **REPLICASET manages a 👇** |
| **POD is an abstraction over 👇** |
| **CONTAINER** |

Abstraction Layers example:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ed9b483e-c722-461d-a89a-c0829c58dac1">

---

**Example:**  
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

### Kubernetes networking
Assume you have a cluster with one control plane and two nodes. When you add nodes to Kubernetes, an IP address is automatically assigned to each node from an internal private network range. For example, assume that your local network range is `192.168.1.0/24`.

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a3894369-3f70-4240-9346-1e88d7771a3f">

Each pod that you deploy gets assigned an IP from a pool of IP addresses. For example, assume that your configuration uses the 10.32.0.0/12 network range, as the following image shows.

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/bb814554-897a-457e-9c8d-4d959afe57b9">

By default, the pods and nodes can't communicate with each other by using different IP address ranges.

To further complicate matters, recall that pods are transient. The pod's IP address is temporary, and can't be used to reconnect to a newly created pod. This configuration affects how your app communicates to its internal components and how you and services interact with it externally.

To simplify communication, Kubernetes expects you to configure networking in such a way that:

1. Pods can communicate with one another across nodes without Network Address Translation (NAT).
2. Nodes can communicate with all pods, and vice versa, without NAT.
3. Agents on a node can communicate with all nodes and pods.

Kubernetes offers several networking options that you can install to configure networking. Examples include Antrea, Cisco Application Centric Infrastructure (ACI), Cilium, Flannel, Kubenet, VMware NSX-T, and Weave Net.

Cloud providers also provide their own networking solutions. For example, Azure Kubernetes Service (AKS) supports the Azure Virtual Network container network interface (CNI), Kubenet, Flannel, Cilium, and Antrea.

---

#### Explanation on `192.168.1.0/24`
Imagine you live in a big apartment building. This building is like your network. Each apartment in the building is like an IP address - it’s a specific location in the network.

The building has 32 floors (8bits.8bits.8bits.8bits) but 24 floors are used to identify the building (network) itself. The remaining 8 floors (bits) are used for the apartments (hosts) within that building (network). 2^8 = 256 IP addresses.

In Computer Networks language, `192.168.1.0/24` represents a subnet with a netmask of `255.255.255.0`. This means that the subnet can provide up 256 IP addresses, ranging from `192.168.1.0` to `192.168.1.255`.  
However, in practice, the first address of a subnet (192.168.1.0 in this case) is reserved for the network address, and the last address (`192.168.1.255`) is reserved for the broadcast address.  
Therefore, there are typically 254 usable IP addresses (`192.168.1.1` through `192.168.1.254`) for hosts in this subnet.

#### Explanation on `10.32.0.0/12`
Keep 12 bits to identify the network and use 20 bits for the hosts.  
So first octet (8 bits) = 10. We want to figure out last 4 bits of the second octet because that's what's used for the hosts.  
2^4 = 16, so that's what's needed to be added to `.32` to find max in second octet. 32 + 15 (0-15 is 16 when 0 is included) = 47.  
So the IP range is: `10.32.0.0` to `10.47.255.255`.

### Kubernetes Services
A K8s service is a K8s object that provides stable networking for pods. A Kubernetes service enables communication between nodes, pods, and users of your app, both internal and external, to the cluster.

Service also provides load balancing.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/73075614-7b57-415d-85ab-73f0e0be0833">

To make your app accessible through the browser, you have to create an external service. External service opens communication from external sources into your cluster.

In the example above, you can't access `db-service` from outside the cluster because it's an internal service.

### Kubernetes Ingress
The request goes to the Ingress which forwards it to the Service.
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/12cc8886-4592-41f3-a543-280e286fafbc">

Notice how the way you access the app using Ingress (_https://my-app.com_) differs from using External Service (_http://124.89.101.2:8080_). [Reference](https://youtu.be/X48VuDVv0do?si=lGg7EqqRyhqnqDcH&t=613).

This is explained in more detail in its own section below.

### Group Pods using Selector
Managing pods by IP address isn't practical. Pod IP addresses change as controllers re-create them, and you might have any number of pods running.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d200b5cc-9adf-4472-9389-07bbd55323ef">

A service object allows you to target and manage specific pods in your cluster by using selector labels. You set the selector label in a service definition to match the pod label defined in the pod's definition file.

For example, assume that you have many running pods. Only a few of these pods are on the front end, and you want to set a LoadBalancer service that targets only the front-end pods. You can apply your service to expose these pods by referencing the pod label as a selector value in the `drone-front-end-service` service's definition file. The service groups only the pods that match the label. If a pod is removed and re-created, the new pod is automatically added to the service group through its matching label.

### ConfigMap
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f1f1f3c4-285e-4d01-9996-0a9412a2c2f3">

### Secrets
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f355abc5-222e-4ff8-9ebe-f1c80a230b43">

### Kubernetes storage
Kubernetes uses the same storage volume concept that you find when using Docker. Docker volumes are less managed than the Kubernetes volumes, because Docker volume lifetimes aren't managed. The Kubernetes volume's lifetime is an explicit lifetime that matches the pod's lifetime. This lifetime match means a volume outlives the containers that run in the pod. However, if the pod is removed, so is the volume.

<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1bcdf8e2-dbe4-4775-8381-943fb1932eef">

Kubernetes provides options to provision persistent storage with the use of PersistentVolumes. You can also request specific storage for pods by using PersistentVolumeClaims.

Keep both of these options in mind when you're deploying app components that require persisted storage, like message queues and databases.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0b2bf7c7-d4e3-4c4f-868b-a8fe89b4c3fa">

### Cloud integration considerations
Kubernetes doesn't provide any of the following services:

1. Middleware
2. Data-processing frameworks
3. Databases
4. Caches
5. Cluster storage systems

In this drone-tracking solution, there are three services that provide middleware functionality: a NoSQL database, an in-memory cache service, and a message queue.

When you're using a cloud environment such as Azure, it's a best practice to use services outside the Kubernetes cluster. This decision can simplify the cluster's configuration and management. For example, you can use Azure Cache for Redis for the in-memory caching services, Azure Service Bus messaging for the message queue, and Azure Cosmos DB for the NoSQL database.

### Explore K8s cluster 
Take a look at [this](https://learn.microsoft.com/en-us/training/modules/intro-to-kubernetes/5-exercise-kubernetes-functionality?pivots=macos).

#### Install MicroK8s
To run MicroK8s on macOS, use Multipass. Multipass is a lightweight VM manager for Linux, Windows, and macOS.

1. `brew install --cask multipass`
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0d0cc10e-02c3-4dc2-99c1-ff9bf2cc2438">
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
2. Check the nodes running in your cluster.
   
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

1. Create deployment
   
   Specify the name of the deployment and the container imahe to create a single instance of the pod.
   
   `sudo kubectl create deployment nginxdeploy --image=nginx`
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/465372c3-3d3a-4333-9541-9653f6ed0d3b">
2. View deployments
   
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/aae09952-8ea0-44e3-97a8-e26323549ee0">
3. View Pods
   
   The deployment from earlier created a pod. View it.
   
   `sudo kubectl get pods`
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/313d83fd-1872-4695-9774-76a5deb90be0">  
   Notice the name of the pod is generated using the deployment name I gave earlier.

#### Test the website installation
1. Find the address of the pod
   
   `sudo kubectl get pods -o wide`
   
   <img width="950" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a8fe7f88-f81b-4a9a-ae8a-ef3e345ed74e">

   Notice that the command returns both the IP address of the pod and the node name on which the workload is scheduled.
2. Access the website
   
   `wget 10.1.254.73`
   
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/473b0a7e-4931-4363-b597-dc9f2675892a">

#### Scale the webserver deployment on a cluster
To scale the number of replicas in your deployment, run the `kubectl scale` command. You specify the number of replicas you need and the name of the deployment.

1. Scale NGINX pods to 3
   
   `sudo kubectl scale --replicas=3 deployments/nginxdeploy`
    
   <img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c2ca06bb-0597-4072-9b70-5abe18c4801d">
2. Check the pods
   
   <img width="900" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/36e1b878-4250-4a63-9541-3411a38ae583">

You'd need to apply several more configurations to the cluster to effectively expose your website as a public-facing website. Examples include installing a load balancer and mapping node IP addresses. This type of configuration forms part of advanced aspects that you'll explore in the future.

### When to use and not use Kubernetes
#### Use Kubernetes
You want to use Kubernetes when your company:  
1. Develops apps as microservices.
2. Develops apps as cloud-native applications.
3. Deploys microservices by using containers.
4. Updates containers at scale.
5. Requires centralized container networking and storage management.

#### Don't use Kubernetes
For example, the effort in containerization and deployment of a monolithic app might be more than the benefits of running the app in Kubernetes. A monolithic architecture can't easily use features such as individual component scaling or updates.

### Uninstall MicroK8s
Before uninstall:  
<img width="200" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/48d515fe-2e7c-4e36-b324-8c39876d9ac0">

1. Remove add ons from the cluster
   
   `sudo microk8s.disable dashboard dns registry`
2. Remove MicroK8s from the VM
   
   `sudo snap remove microk8s`
3. Exit the VM
   
   `exit`
   
   <img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2add3d14-da0d-4dea-beba-fa0ce5a92312">
4. Stop the VM
   
   `multipass stop microk8s-vm`
5. Delete and purge the VM instance
   
   ````
   multipass delete microk8s-vm
   multipass purge
   ````

After uninstall:  
<img width="200" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5fe17dad-8cd3-45ec-ac17-980b00ac126d">

---

## Nana's Kubernetes course
[Reference](https://youtu.be/X48VuDVv0do?si=LBqvoUujADAQHj_n).

### More `kubectl` commands
1. Get Logs from Pod
   ````
   kubectl logs [pd name]
   ````
2. Describe Pod
   ````
   kubectl describe pod [pod name]
   ````
3. Debugging by getting into the pod and get the terminal (bash for eg.) from in there
   ````
   kubectl exec -it [pod name] -- bin/bash
   ````
   
   <img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/789efdfc-1d77-4bfe-8392-24b433c70c98">
4. Delete Pods
   ````
   kubectl delete deployment [name]
   ````
   
   <img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b549874e-87a1-4fd6-ba1e-fc2625d0e5d8">
5. Using config files to do deployments
   ````
   kubectl apply -f [some-config-file.yaml]
   ````
   
   The first `spec` is for deployment and the second `spec` is for pods.  `template` is the blueprint for the pods.
   
   <img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/624db0b7-1660-49e2-9dd2-056f6ed9b962">

**Summary:**  
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/847d015b-1085-459f-94d5-22b7f48c3559">  
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/54533231-60fe-4f3f-98c9-504bb5ad2d71">

### Kubernetes YAML config file
YAML is a human friendly data serialization standard for all programming languages.  
Its syntax uses strict indentation.  
Best practice is to store config file with your code (or have a separate repo for config files).

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/754f9518-250a-4802-8720-9cd1521f6c0b">

Each configuration file has 3 parts:  
1. Metadata
2. Specification
3. Status (This part is automatically generated and added by K8s)  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/333a0dc3-e0a7-4879-b857-5b0c679679c5">

   K8s gets status data from `etcd` database.
   `etcd` holds the current status of any K8s component.

#### Blueprint for Pods (Template)
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2fd8c270-f25b-4be1-a802-0e565390599d">

#### Connecting Components (Labels & Selectors & Ports)
`metadata` part contains labels and `spec` part contains selectors.

1. Connecting Deployment to Pods  
   Way for deployment to know which Pods belong to it.
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/eaa3face-37d5-4149-8963-00b5b08b2549">

   Pods get the label through the template blueprint.  
   Then we tell the deployment to connect or to match all the labels `app: nginx`.

2. Connecting Services to Deployments  
   Way for service to know which Pods belong to it.
   
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5ae2b2a1-f84b-48db-bf8a-25be3247c6c2">

   Example:  
   Service has `spec:ports` configuration and deployment has `spec:template:spec:containers:ports` configuration:  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8fcb98bd-fc49-4acc-93b3-970148742ed8">

#### Ports in Service and Pod
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fbcaa9c7-61a1-498c-864f-0f71af2d10e6">

#### Example
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/21ec293e-4579-44d5-907e-acff811b436b">

#### View the status of the deployment generated by K8s from `etcd` and compare it to original
````
kubectl get deployment nginx-deployment -o yaml > nginx-deployment-result.yaml
````

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a7aea357-a4f9-4ab1-a81e-c470a5dc3060">

#### Delete the deployment and service
````
kubectl delete -f nginx-deployment.yaml  
kubectl delete -f nginx-service.yaml
````

### Elaborate [example](https://youtu.be/X48VuDVv0do?si=-WTbSOZ04U-VLaCJ&t=4576)
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c0420032-d53b-4a61-8fab-65de95579945">

#### Create MongoDb deployment
Create secret first before you run deployment. Secret is created in the section that immediately follows this one.
````
mongodb-deployment.yaml
````

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
````
[~]$ kubectl apply -f mongodb-deployment.yaml
````

#### Create secret (this will live in K8s, not in repo)
Create secret file:
````
mongodb-secret.yaml
````

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
````
[~]$ cd k8s-config/
[~]$ ls
mongodb-deployment.yaml      mongo.yaml
[~]$ kubectl apply -f mongodb-secret.yaml
[~]$ secret/mongodb-secret created
````

`k8s-config` is just a folder on your local computer.

You can view secrets with command:
````
kubectl get secret
````

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
````
[~]$ kubectl apply -f mongodb-deployment.yaml
deployment.apps/mongodb-deployment unchanged
service/mongodb-service created
````

To check that the service is attached to the correct pod:
````
kubectl describe service mongodb-service
````

<img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1ef6b94c-27cc-42ea-a4bd-69835d7011cc">

That's the POD IP address and `27017` is the port where the application inside the Pod is listening.

Check that IP on the Pod by running:
````
kubectl get pod -o wide
````

#### View all components created so far
````
kubectl get all | grep mongodb
````

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b6c20141-e2ed-4712-80d1-05dc73912828">

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
````
kubectl apply -f mongodb-configmap.yaml
````

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
````
kubectl apply -f mongoexpress-deployment.yaml
````

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
````
kubectl apply -f mongoexpress-deployment.yaml
````

Check it out:  
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ac0b67c1-2c53-4300-b776-ca515fbf6815">

Internal Service or Cluster IP is DEFAULT.  
LoadBalancer is also assigned an External IP.

#### Give a URL to external service in minikube
````
minikube service mongo-express-service
````

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/884d849b-96a5-409f-87d4-37e48eb82ce8">

### Namespace
Way to organize resources.

You can put your object/ resource into some namespace in the .yaml configuration file.  
<img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6b58171f-c4a1-4863-91f4-80f5b7812987">

**Why?**

1. To group resources into different namespaces.  
   <img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/699c0798-624e-4374-8fe5-18af861fef02">

2. Use same cluster by different teams.  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6d2809d6-83b0-49cd-aceb-dc40f09f4e17">

3. Resource sharing: Staging and Development.  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/5da2c1b1-931e-4796-b972-570a517ef9a2">

4. Set access and resource limits.  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/80a7b5cb-fbbb-4f24-9aa6-9575d4944bc9">

You **can access** service in another Namespace:

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d427f54a-b0f1-4623-81ce-2ec6bcf910f6">

You **can't access** most resources from another namespace.  
For eg: Each namespace must define its own ConfigMap, Secret etc.

There are some components that can't live inside a namespace.  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0a224443-b673-45b4-829e-b49a7749c7de">

### Ingress
To make an app accessible through the browser, you can use external service or ingress.

External Service looks like below. It uses http protocol and IP address:port of the node which is ok for testing but not good for final product.  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f43a4b47-8075-4ef3-bb91-269b7239b503">

Ingress makes it better:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/08c8836d-d372-4c37-888a-80a9cb875635">

One thing to note is that the `http` attribute under `rules` doesn't correspond to http protocol in the browser.  
This just means that the incoming request gets forwarded to internal service.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a78853c4-b1ee-4082-99b1-6d9e484fd783">

#### Ingress and internal service configuration
<img width="750" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/88972574-7f1d-426f-8caa-b4df5b6065d8">

In the `host: myapp.com`, `myapp.com` must be a valid domain address.  
You should map domain name to Node's IP address, which is the entrypoint. 

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f23b89da-c00e-451f-9ff5-686cc5c47764">

#### How to configure ingress in your cluster
You need an implementation for Ingress which is called Ingress Controller.

Install an ingress controller which is basically a pod or a set of pods which run on your node in your K8s cluster.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6edf9cb0-bbf1-4539-aee7-383903e7a45b">

#### Ingress Controller
1. Evaluates all the rules defined in the cluster.  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/b6e2ba98-c58b-4999-b773-2e87c3ac8705">
2. Manages redirections.
3. Entrypoint to cluster.
4. Many third-party implementations.

If you're running your K8s cluster on some cloud environment, you'll already have a cloud load balancer.
External requests will first hit the load balancer and that will redirect the request to Ingress Controller. 

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e8228d68-cadd-4970-b66c-510223223ca8">

But if you're deploying your K8s cluster on a bare-metal, you'll have to configure some kind of entrypoint yourself. That entrypoint can be either inside of your cluster or outside as a separate server.  
For eg: An external Proxy Server shown below.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8499ad31-76a4-41e1-ae5d-0924cef3b327">

#### Install Ingress Controller in Minikube
````
minikube addons enable ingress
````

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d27794bd-aedc-4e65-906e-b914881c8c71">

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
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6d78b75f-bb8a-44c0-b4e8-bc6111e88214">

#### Multiple sub-domains or domains
You can also have multiple hosts with 1 path. Each host represents a subdomain.

<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e0e61b6e-ab95-4335-8215-64a18fb42727">

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

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/9e221f9d-e32f-43b1-94c6-7aaae9f8a1af">

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
````
helm install --values=my-values.yaml <chartname>
````

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/039605b5-2e17-4e06-850f-17445683f955">

#### Helm helps deploying same app across different environments
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e0d3f594-f21a-4b81-a733-b5318a3c695d">

#### Helm chart structure
<img width="150" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/124b26be-9dae-4b47-9e93-e162c474195f">

1. `mychart` folder: Name of the chart.
2. `Chart.yaml`: Meta info about the chart. For eg: name, version, dependencies etc.
3. `values.yaml`: Values for the template files.
4. `charts` folder: Chart dependencies. For eg: If this chart depends on other charts.
5. `templates` folder: Actual template files.

Install chart using the command:
````
helm install <chartname>
````

#### Helm version 3 got rid of Tiller for good reasons

### Kubernetes Volumes
Persist data in K8s using following ways:  
1. Persistent Volume
2. Persistent Volume Claim
3. Storage Class

Let's say you have an app with a Db.

<img width="350" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/69fa3fb5-0cdc-42f3-9fbd-4a4637765c3b">

If you make changes to the db and restart the `mysql` pod, the data gets lost because K8s doesn't provide data persistence out of the box.

**Storage Requirements:**  
1. We need a storage that doesn't depend on the pod lifecycle.  
   <img width="400" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/7584ed59-ff6d-475b-b705-cffb41323a3d">
2. Storage must be available on all nodes.  
   <img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/94987239-4ba4-46e5-a2c9-56c3f6db9c4b">
3. Storage needs to survive even if cluster crashes.

#### Persistent Volume
1. Cluster resource.
2. Created via YAML file.
3. Needs actual physical storage.  
   <img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e65c8cf7-7d25-47e0-a3c8-e9307869243f">
4. Example that uses Google cloud:  
   <img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/af252f0c-4df7-45d3-90e3-5aec28e68ae4">
5. They are NOT namespaced.  
   <img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ebc1eea6-1336-4c6d-b8ef-bb85f45f2356">

#### Persistent Volume Claim
Application has to claim the Persistent volume.

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1a383fc1-829c-4ea6-acc8-8ff59232e688">

In other words, PVC specifies claims about a volume with storage size, access mode etc. and whichever persistent volume matches those claims will be used for the application.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a2bd62e8-f5d4-40ae-aa48-1673ae95f8b3">

Note: PVClaims must exist in the same namespace as the pod.

You use that claim in the Pods configuration like so:  
<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/fee3ae01-a2e6-48f0-a149-ae3f9823df03">

**Mounting volume:**  
<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4d860872-729b-41e2-ad93-4c4ecde0411c">

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

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c58b7dc3-befe-436c-8bbf-0796aa3567d4">

#### Deployment vs StatefulSet
**Deployment:**

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/e43dcd7d-baf8-4d56-9584-d89475c879ac">

When you delete one, one of those pods can be deleted randomly. 

**StatefulSet:**

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/71a6477b-143f-4d80-aa04-6fcc427b3905">

#### Pod Identity

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/1e3c1181-a506-41c4-8746-6fecee9048b4">

Let's say `ID-2` pod goes down and gets created again. Even when it's created newly this time, it'll have the same Id of `ID-2`.

#### Scaling Db apps
Only let one instance to write data to prevent data inconsistencies.

Let's say you add `mysql-3` instance.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/284d1af5-84ba-48cd-bae7-9328cf9b15a6">

The workers continuously synchronize data.  
PV means Persistent volume.

#### Pod state 
<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/036fb5c8-bcb1-4fa2-9f66-5a38383a0b93">

Pod state is stored in its storage. When a pod dies and it gets replaced, the persistent pod identifiers make sure that the storage volume gets reattached to the replacement pod.

#### Pod Identity
<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/344b5add-a958-45f7-9efd-7dd07905c50b">

For eg: StatefulSet with 3 replica:
1. mysql-0 : Master
2. mysql-1 : Worker
3. mysql-2 : Worker

Next pod is only created if previous one is up and running.  
Deletion in reverse order, starting from the last one.

#### Pod Endpoints
Each pod in a stateful set gets it own DNS endpoint from a service.

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8cb0d78d-8e29-483d-b471-ebf6d66b1199">

2 characteristics:  
<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/a4bbcbc5-21cb-4494-aa64-2aa766578a99">

#### It's complex so just use cloud db services.
Stateful apps are not perfect for containerized apps, stateless are.

### Kubernetes Services
Pods in K8s are destroyed frequently, so it's not practical to reach them using their ip addresses.
Services address this problem by providing a stable IP address even when the pod dies.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f677caad-fd51-47eb-b1cd-943194ddd6c2">

**Different Services:**
1. ClusterIP Services
2. Headless Services
3. NodePort Services
4. LoadBalancer Services

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/2dfe7d6e-90a5-4407-975c-5b2cc5afc79b">

#### ClusterIP Services
This is a default type, so when you create a service and don't specify the type, this is what you'll get.

<img width="300" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c87f3b55-c180-4008-a9a8-a79ab1b299a9">

For eg:  
Let's say we have deployed our microservice app in our cluster. Let's say this pod contains the app container and a sidecar container that collects logs from the app and sends it to some log store. The deployment yaml file looks like this:

<img width="550" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ee490118-8260-400e-b21a-7a20c0e78557">

**Pod gets an IP address from a range that is assigned to a Node.**

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/d48f38c7-1f29-4667-9adc-60501d235eb7">

Let's say you access the app through the browser:

<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/cbf516f7-4038-4d7f-b443-63db0cb4f932">

The ingress rule and app service yaml contents look like this:

<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/8fa1242c-a1f5-485a-b81c-baaf5101cf80">  

As you can see, the ingress forwards to port `3200` where the service is, and the service forwards to port `3000` in the container where the app is running.

The `servicePort` is arbitrary while `targetPort` is not. `targetPort` has to match the port where container inside the pod is listening at.

**Which pods to forward request to?**  
By using selectors which are labels of pods.

<img width="500" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f449dc1d-2fbb-4aba-b1b9-c0d9feb2186c">

**Which port to forward request to?**  
<img width="800" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/3cb110d9-424d-4988-a028-e3cbc6cf920e">

When we create the service, it will find all the pods that match the selector (1), so these pods will become **endpoints** of the service.
When service gets a request, it will pick one of those pod replicas randomly beause it's a load balancer and it will send the request it received to that specific pod on a port defined by `targetPort` attribute (2).

**Service Endpoints:**  
When we create a service, K8s creates an endpoints object that has the same name as the service and it will use the endpoints object to keep track of which pods are members/ endpoints of the service.

<img width="450" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/6f335791-f1c7-4877-a1e6-9d7d52bb7136">

**Multi-Port Services:**  
<img width="800" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/f04cb733-52ad-43d1-a835-499867807101">

When you have multiple ports defined in a service, you have to name them.

<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/32f013d4-2240-461d-a0c5-829478b9e1c0">

#### Headless Services
Useful in scenario like:
1. Client wants to communicate with 1 specific Pod directly.
2. Pods wants to talk directly with specific Pod.
3. Pod can't be randomly selected.

**Use case:** Stateful applications, like databases where Pod replicas aren't identical.

To create a headless service, set clusterIP to none.

<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/48b67607-a0fb-4652-a069-7150e9b88233">

We have these 2 services alongside each other. ClusterIP one will handle load balanced requests, Headless one will handle data synchronization requests.

<img width="800" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ae95e98b-5227-4a7d-a7af-4a2e5f7925fe">

#### NodePort Services
Recall that ClusterIP is only accessible within the cluster so no external traffic can directly address the cluster IP service.
The node port service however makes the external traffic accessible on static or fixed port on each worker node.

So in this case, instead of a browser request coming through the Ingress, it can directly come to the worker node at the port that the service specification defines.
This way external traffic has access to fixed port on each Worker Node.

This is how NodePort service is created.
Whenever we create a node port service, a cluster ip service to which the node port service will route is automatically created.

<img width="650" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/ec3c57fc-2628-4cb9-82b7-5a69fb8f7f0a">

The nodePort value has a predefined range of `30000 - 32767`.  
Here you can reach the app at `172.90.1.2:30008`.

**View the service:**

The node port has the cluster IP address and for that IP address, it also has the port open where the service is accessible at.  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/941eb8bd-0a3b-44cf-b3f4-7cfe7ed4aa47">

**Service spans all the worker nodes:**  

Service is able to handle a request coming on any of the worker nodes and then forward it to one of the pod replicas. Like `172.90.1.1:30008` or `172.90.1.2:30008` or `172.90.1.3:30008`. This type of service exposure is not secure.  
<img width="600" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4ea4d459-2053-4784-8eb6-713b9936d3ef">

Don't use it for external connection. Use it only for testing.

Configure Ingress or LoadBalancer for production environments.

#### LoadBalancer Services
A service becomes accessible externally through a cloud provider's load balancer service. 

LoadBalancer service is an extension of NodePort service.  
NodePort service is an extension of ClusterIP service.  
So whenever we create LoadBalancer service, NodePort and ClusterIP services are created automatically.

The YAML file looks like this:  
<img width="250" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/0e41ac26-8297-4c25-8881-59b261b37261">

It works like this:  
<img width="700" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/c717755a-24ad-4a2d-98bb-39b9391906d1">

NodePort is the port that is open on the worker node but it's not directly accessible externally but only through the load balancer. 
So the entry point becomes the load balancer first and it can then direct the traffic to node port on the worker node and the cluster IP, the internal service.

The load balancer service looks like this:  
<img width="800" alt="image" src="https://github.com/affableashish/k8s-hands-on/assets/30603497/4706aeb2-8daf-4364-a820-ede12058f9c9">

Configure Ingress or LoadBalancer for production environments.

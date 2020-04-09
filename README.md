[![dockeri.co](https://dockeri.co/image/coolexplorer/kube-consul-registrator)](https://hub.docker.com/r/coolexplorer/kube-consul-registrator)

![.NET Core](https://github.com/coolexplorer/kube-consul-registrator/workflows/.NET%20Core/badge.svg)
[![CodeFactor](https://www.codefactor.io/repository/github/coolexplorer/kube-consul-registrator/badge)](https://www.codefactor.io/repository/github/coolexplorer/kube-consul-registrator)
[![codecov](https://codecov.io/gh/coolexplorer/kube-consul-registrator/branch/master/graph/badge.svg)](https://codecov.io/gh/coolexplorer/kube-consul-registrator)
![release](https://badgen.net/github/release/coolexplorer/kube-consul-registrator)
![docker](https://github.com/coolexplorer/kube-consul-registrator/workflows/docker/badge.svg)

# kube-consul-registrator

## What is the kub-consul-registrator?

[kube-consul-registrator](https://github.com/allenkim-qe/kube-consul-registrator) is the service that can handle consul registration and deregistration while kube-consul-registrator run into the Kubernetes cluster.

## Getting Started

### Prerequisites

- Kubernetes cluster
- Consul server : [docker-compose example](/docker/consul/docker-compose.yml)

### Environment variables

| Envrionment variable | Explaination | Example |
|---|:---:|:---:|
| CONSUL_ADDRESS | Consul address in the cluster |  "consul:8500" |
| KUBE_ALLOWED_NAMESPACES | Namespaces to register pods | "default,namespace1"


### Annotations

These are annotation samples when you want to add a pod in the consul. You should add this annotations on the metadata of a pod. (Do not add this on the metadata of Deployment, ReplicaSet, StatefulSet, and DaemonSet)

```
consul-registrator/enabled: true
consul-registrator/service-id: foo
consul-registrator/service-name: bar
consul-registrator/service-port: 80
consul-registrator/service-meta-<key>:<value>
```
Kubernetes definistion example
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: consul-register-deployment
  namespace: default
spec:
  selector:
    matchLabels:
      component: consul-register
  replicas: 1
  template:
    metadata:
      labels:
        component: consul-register
      annotations:
        consul-registrator/enabled: true
        consul-registrator/service-id: foo
        consul-registrator/service-name: bar
        consul-registrator/service-port: 80
        consul-registrator/service-meta-type:test           # add metadata [type:test]
    spec:
      serviceAccountName: consul-register
      containers:
      - name: consul-register
        image: coolexplorer/kube-consul-registrator:latest
        imagePullPolicy: Always
        ports:
        - containerPort: 80
        env:
        - name: CONSUL_ADDRESS
          value: "consul:8500"
        - name: KUBE_ALLOWED_NAMESPACES
          value: "qe-tools,monitoring"
        - name : Logging__LogLevel__Default
          value: "Debug"
```

### Installing

To use this service, you need to create kube-consul-registrator pod with authorization to get the cluster information from the cluster. Please refer to the files that I added in the repository.
- [Deployment definition](k8s/kube-consul-registrator.yaml)
- [Service definition](k8s/kube-consul-registrator.yaml)
- [Service Account definition](k8s/kube-consul-registrator.yaml)
- [ClusterRoleBinding definition](k8s/kube-consul-registrator.yaml)
- [ClusterRole](k8s/kube-consul-registrator.yaml)

## Built With

- [Kubernetes C# Client](https://github.com/kubernetes-client/csharp) 
- [Consul client](https://github.com/PlayFab/consuldotnet)

## Authors
- Allen Kim - Initial work - [coolexplorer](https://github.com/coolexplorer)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

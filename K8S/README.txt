--Create Secret
kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"

--Create Ingress Nginx
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.11.2/deploy/static/provider/cloud/deploy.yaml
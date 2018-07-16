FROM  microsoft/dotnet:aspnetcore-runtime
LABEL Author="Hamza Althunibat <althunibat@outlook.com>"

WORKDIR /app
COPY ./out/ .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV TZ=Asia/Dubai
EXPOSE 80
RUN mkdir -p /app/logs
HEALTHCHECK --interval=15s --timeout=3s CMD curl -f http://localhost/hc || exit 1
ENTRYPOINT [ "dotnet","Identity.Server.dll" ]
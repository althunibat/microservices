FROM  microsoft/dotnet:aspnetcore-runtime
LABEL Author="Hamza Althunibat <hamza.althunibat@moe.gov.ae>"

WORKDIR /app
COPY ./out/ .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://+:80
ENV TZ=Asia/Dubai
EXPOSE 80
RUN mkdir -p /app/logs
HEALTHCHECK  CMD curl -f http://localhost/hc || exit 1
ENTRYPOINT [ "dotnet","SampleApi.dll" ]
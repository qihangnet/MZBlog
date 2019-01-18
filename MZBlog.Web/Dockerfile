FROM microsoft/dotnet:2.2-aspnetcore-runtime
ENV TZ Asia/Shanghai
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MZBlog.Web.dll"]
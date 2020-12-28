FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
RUN apk add --no-cache tzdata
ENV TZ Asia/Shanghai
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MZBlog.Web.dll"]
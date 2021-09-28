FROM mcr.microsoft.com/dotnet/sdk:5.0 as dotnetbuild

WORKDIR /build/backend
COPY Luger .
RUN dotnet restore && dotnet publish --output ./publish --configuration Release

FROM node:16-alpine as reactbuild
WORKDIR /build/frontend
COPY Luger.React .
RUN yarn && yarn build

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as runtime
WORKDIR /luger
COPY --from=dotnetbuild /build/backend/publish .
COPY --from=reactbuild /build/frontend/build wwwroot

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV Luger__Users__0__Id="admin"
ENV Luger__Users__0__Password="admin"
ENV Luger__Users__0__Buckets__0="bucket"
ENV Luger__Buckets__0__Id="bucket"
ENV Jwt__SigningKey="My secred password for JWT"

EXPOSE 7931

ENTRYPOINT [ "dotnet", "Luger.dll" ]

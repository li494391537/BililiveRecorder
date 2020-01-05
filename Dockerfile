FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS builder
COPY BililiveRecorder.Core /build/BililiveRecorder.Core
COPY BililiveRecorder.FlvProcessor /build/BililiveRecorder.FlvProcessor
COPY ConsoleApp1 /build/ConsoleApp1
WORKDIR /build/ConsoleApp1
RUN mkdir /build/out && dotnet build -o /build/out -c RELEASE

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine
COPY --from=builder /build/out/* /bili
RUN mkdir /bili/data
WORKDIR /bili/data
VOLUME [ "/bili/data" ]
ENTRYPOINT ["dotnet", "../ConsoleApp1.dll", "-i"]
CMD ["528819"]
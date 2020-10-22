.PHONY: build build-no-cache

build:
	cd .. && \
	docker build -t ssrd/securebank -f dockerimage/Dockerfile .

build-no-cache:
	cd .. && \
	docker build -t ssrd/securebank -f dockerimage/Dockerfile --no-cache .

#!/usr/bin/env bash

docker compose --env-file ../../env/.env.prod --profile prod --profile monitoring up -d --build
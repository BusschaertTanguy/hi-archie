module.exports = {
    api: {
        output: {
            target: "src/api/types/index.ts",
            client: "react-query",
            prettier: true,
            override: {
                useNativeEnums: true,
                mutator: {
                    path: "src/api/mutator/axios-instance.ts",
                    name: "axiosInstance",
                },
            },
        },
        input: "http://localhost:5400/openapi/v1.json",
    },
};
<template>
  <v-app id="inspire">
    <v-content>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <router-view></router-view>
          </v-flex>
          <vue-snotify></vue-snotify>
        </v-layout>
      </v-container>
    </v-content>
  </v-app>
</template>

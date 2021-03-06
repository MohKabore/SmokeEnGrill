import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from 'src/app/shared/models/user';
import { Setting } from 'src/app/shared/models/setting';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl + 'admin/';
  sexe = [{ id: 0, name: ' FEMME' }, { id: 1, name: ' HOMME' }];

  constructor(private http: HttpClient) { }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'editRoles/' + user.userName, roles);
  }

  getPhotosForApproval() {
    return this.http.get(this.baseUrl + 'photosForModeration');
  }

  approvePhoto(photoId) {
    return this.http.post(this.baseUrl + 'approvePhoto/' + photoId, {});
  }

  rejectPhoto(photoId) {
    return this.http.post(this.baseUrl + 'rejectPhoto/' + photoId, {});
  }

  emailExist(email: string) {
    return this.http.get(this.baseUrl + email + '/VerifyEmail');
  }

  userNameExist(userName: string) {
    return this.http.get(this.baseUrl + userName + '/VerifyUserName');
  }

  searchUsers(data: any) {
    return this.http.post(this.baseUrl + 'SearchUsers', data);
  }

  // recherche des incription en fonction du niveau, du nom ou du prenom
  searchIncription(searchParams) {
    let params = new HttpParams();
    params = params.append('levelId', searchParams.levelId);
    params = params.append('lastName', searchParams.lastName);
    params = params.append('firstName', searchParams.firstName);

    return this.http.get(this.baseUrl + 'SearchInscription', { params });
  }

  studentAffectation(classid, ids) {
    return this.http.post(this.baseUrl + classid + '/StudentAffectation', ids);
  }

  // enregistrement des professeurs extraits du fichier excel
  importTeachersFile(teachers: User[]) {
    return this.http.post(this.baseUrl + 'ImportTeachers', teachers);
  }

  saveImportedUsers(importedUsers, insertUserId) {
    return this.http.post(this.baseUrl + 'ImportedUsers/' + insertUserId, importedUsers);
  }

  getSettings() {
    return this.http.get(this.baseUrl + 'Settings');
  }

  updateSettings(settings: Setting[]) {
    return this.http.post(this.baseUrl + 'UpdateSettings', settings);
  }

  getRoles() {
    return this.http.get(this.baseUrl + 'Roles');
  }

  getEmpData() {
    return this.http.get(this.baseUrl + 'EmpData');
  }

  getRoleWithUsers(roleId) {
    return this.http.get(this.baseUrl + roleId + '/RoleWithUsers');
  }

  getRolesWithUsers() {
    return this.http.get(this.baseUrl + 'RolesWithUsers');
  }

  getUserTypeMenu(userTypeId) {
    return this.http.get(this.baseUrl + 'LoadMenu/' + userTypeId);
  }

  getMenuWithCapabilities(userTypeId) {
    return this.http.get(this.baseUrl + 'GetMenuCapabilities/' + userTypeId);
  }

  saveRole(role) {
    return this.http.post(this.baseUrl + 'saveRole', role);
  }

  getEmployees() {
    return this.http.get(this.baseUrl + 'employees');
  }

  getDistricts() {
    return this.http.get(this.baseUrl + 'Districts');
  }

  getRoleEmployees(roleId) {
    return this.http.get(this.baseUrl + 'RoleEmployees/' + roleId);
  }

  getTuitionFees() {
    return this.http.get(this.baseUrl + 'TuitionFees');
  }

  saveLevelTuitionFees(tuitionFeeData) {
    return this.http.post(this.baseUrl + 'saveLevelTuitionFees', tuitionFeeData);
  }

  saveService(service) {
    return this.http.post(this.baseUrl + 'SaveService', service);
  }

  getZonesWithLocations() {
    return this.http.get(this.baseUrl + 'ZonesWithLocations');
  }

  getZones() {
    return this.http.get(this.baseUrl + 'Zones');
  }

  saveZones(zones) {
    return this.http.post(this.baseUrl + 'saveZones', zones);
  }

  getCountriesData() {
    return this.http.get(this.baseUrl + 'CountriesToDistricts');
  }

  getJobs() {
    return this.http.get(this.baseUrl + 'Jobs');
  }

}

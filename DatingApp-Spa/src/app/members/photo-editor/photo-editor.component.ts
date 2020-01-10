import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
@Input() photos: Photo[];
@Output() updateMemberPhoto = new EventEmitter<string>();
uploader: FileUploader;
hasBaseDropZoneOver: false;
currentActivePhoto: Photo;
baseUrl = environment.apiUrl;
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.inilializeUploader();
  }
  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  inilializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      maxFileSize: 10 * 1024 * 1024,
      removeAfterUpload: true,
      autoUpload: false
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response){
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.decription,
          isActive: res.isActive
        };

        this.photos.push(res);
      }

    };
  }

  setActivePhoto(photo: Photo){
    this.userService.setActivePhoto(this.authService.decodedToken.nameid,photo.id).subscribe(() => {
      this.alertify.success('Photo succesfully set to active.');
      this.currentActivePhoto = this.photos.filter(p => p.isActive === true)[0];
      this.currentActivePhoto.isActive = false;
      photo.isActive = true;
      this.updateMemberPhoto.emit(photo.url);
      this.authService.currentUser.photoURL = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto(id: number){
    this.alertify.confirm('Are you sure you want to delete the photo.', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() =>{
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertify.success('Photo deleted successfully');
      }, error => {
        this.alertify.error(error);
      });
    });
  }

}

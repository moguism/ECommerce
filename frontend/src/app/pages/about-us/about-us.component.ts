import { Component, ViewChild, ElementRef } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
import { Member } from '../../models/member';
import * as THREE from 'three';

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './about-us.component.html',
  styleUrl: './about-us.component.css'
})
export class AboutUsComponent {
  @ViewChild('objeto3d', { static: true }) objeto3d: ElementRef | undefined;

  private scene: THREE.Scene;
  private camera: THREE.PerspectiveCamera | undefined;
  private renderer: THREE.WebGLRenderer | undefined;
  private model: THREE.Object3D | undefined;
  private light: THREE.DirectionalLight | undefined;
  private light2: THREE.DirectionalLight | undefined;


  constructor() {
    this.scene = new THREE.Scene();
  }
  ngAfterViewInit() {
    this.init3Dscene()
    this.animate()
}

init3Dscene() {
  this.camera = new THREE.PerspectiveCamera( 75, 500 / 500, 1, 1000 );
  this.camera.position.z=5;
  this.camera.position.y=0;
  this.renderer=new THREE.WebGLRenderer({
    antialias: true,
    alpha: true
});
if(screen.width <= 500){
  this.renderer.setSize(300,300);
}
else{
  this.renderer.setSize(500, 500);
}
    if (this.objeto3d?.nativeElement) {
      this.objeto3d.nativeElement.appendChild(this.renderer.domElement);
    }

    
    const color = 0xffffff;
    const intensity = 4;
    this.light = new THREE.DirectionalLight(color, intensity);
    this.light2 = new THREE.DirectionalLight(color,intensity)

      this.light.position.set(12.991, 3.531, 100);
      this.light2.position.set(12.991, 3.531, -100);
      this.scene.add(this.light);
      this.scene.add(this.light2);

   
    const loaderGLTF = new GLTFLoader();
    loaderGLTF.load('assets/object-3d/scene.gltf', (gltf) => {
      this.model = gltf.scene;
        this.scene.add(this.model);
        this.model.position.set(0, 0, 0);
    });
    
    
    const controls = new OrbitControls(this.camera, this.renderer.domElement);

   
  }

  private animate(): void {
    requestAnimationFrame(() => this.animate());

    if (this.model) {
      this.model.rotation.y += 0.01;
    }

    if (this.renderer && this.camera) {
      this.renderer.render(this.scene!, this.camera);
    }
  }
}


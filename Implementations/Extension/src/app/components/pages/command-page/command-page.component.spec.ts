import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommandPageComponent } from './command-page.component';

describe('CommandPageComponent', () => {
  let component: CommandPageComponent;
  let fixture: ComponentFixture<CommandPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CommandPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CommandPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
